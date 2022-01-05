using System.Net;
using Microsoft.AspNetCore.Mvc;
using RecyclingCalendar.Core.DTO;

namespace RecyclingCalendar.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StreetsController: ControllerBase
{
    [HttpGet("~/zip-codes/{zipCode}/streets")]
    [ProducesResponseType(typeof(IList<Street>), (int) HttpStatusCode.OK)]
    public IActionResult GetStreets(string zipCode)
    {
        return Ok(new List<Street>());
    }
}