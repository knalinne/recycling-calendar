using System.Net;
using Microsoft.AspNetCore.Mvc;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StreetsController : ControllerBase
{
    private readonly IStreetService _streetService;
    private readonly IZipCodeService _zipCodeService;

    public StreetsController(IStreetService streetService, IZipCodeService zipCodeService)
    {
        _streetService = streetService;
        _zipCodeService = zipCodeService;
    }

    [HttpGet("~/zip-codes/{zipCode}/streets")]
    [ProducesResponseType(typeof(IList<Street>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetStreets(string zipCode)
    {
        var zipCodes = await _zipCodeService.FindByCode(zipCode);
        return zipCodes.Count switch
        {
            > 1 => BadRequest($"zip code {zipCode} is not unique"),
            0 => BadRequest($"zip code {zipCode} not found"),
            _ => Ok(await _streetService.FindAll(zipCodes.First().Id))
        };
    }
}