using System.Net;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Mvc;
using RecyclingCalendar.Api.Models;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Api.Controllers;

[ApiController]
public class EventController : ControllerBase
{
    private readonly IZipCodeService _zipCodeService;
    private readonly IStreetService _streetService;
    private readonly IRecyclingEventService _recyclingEventService;

    public EventController(IZipCodeService zipCodeService, IStreetService streetService,
        IRecyclingEventService recyclingEventService)
    {
        _zipCodeService = zipCodeService;
        _streetService = streetService;
        _recyclingEventService = recyclingEventService;
    }

    [HttpGet("~/{zipCode}/{street}/{houseNumber:int?}")]
    [ProducesResponseType(typeof(IList<RecyclingEvent>), (int)HttpStatusCode.OK, "text/calendar", "application/json")]
    public async Task<IActionResult> GetCalendar([FromQuery] EventsOptions options, string zipCode, string street, int houseNumber = 1)
    {
        var zipCodes = await _zipCodeService.FindByCode(zipCode);
        switch (zipCodes.Count)
        {
            case > 1:
                return BadRequest($"zip code {zipCode} is not unique");
            case 0:
                return BadRequest($"zip code {zipCode} not found");
            default:
            {
                var streets = await _streetService.FindByName(zipCodes.First().Id, street);
                return streets.Count switch
                {
                    > 1 => BadRequest($"street {street} is not unique for zip code {zipCode}"),
                    0 => BadRequest($"street {street} not found for zip code {zipCode}"),
                    _ => await BuildGetCalendarResponse(zipCodes[0].Id, streets[0].Id, houseNumber, options)
                };
            }
        }
    }

    private async Task<IActionResult> BuildGetCalendarResponse(string zipCodeId, string streetId, int houseNumber,
        EventsOptions options)
    {
        var events = await _recyclingEventService.FindBy(zipCodeId, streetId, houseNumber);
        var jsonResponse = Request.Headers.Accept.Any(acceptHeader => acceptHeader == "application/json");
        if (jsonResponse) return Ok(events);

        var serializer = new CalendarSerializer();
        return Ok(serializer.SerializeToString(BuildCalendar(events, options)));
    }

    private static Calendar BuildCalendar(IList<RecyclingEvent> events, EventsOptions options)
    {
        var calendar = new Calendar()
        {
            Name = "Poubelles"
        };
        events.ToList().ForEach(recyclingEvent =>
        {
            var e = new CalendarEvent()
            {
                IsAllDay = true,
                Start = new CalDateTime(recyclingEvent.EventDate.ToDateTime(TimeOnly.MinValue)),
                Summary = recyclingEvent.Summary,
                Description = recyclingEvent.Description,
            };
            if (options.AlarmTime != null)
            {
                var alarmTime = recyclingEvent.EventDate.AddDays(-1).ToDateTime(TimeOnly.Parse(options.AlarmTime));
                e.Alarms.Add(new Alarm()
                {
                    Action = AlarmAction.Display,
                    Trigger = new Trigger(alarmTime - recyclingEvent.EventDate.ToDateTime(TimeOnly.MinValue))
                });
            }
            calendar.Events.Add(e);
        });
        return calendar;
    }
}