using System.Net;
using Microsoft.AspNetCore.Mvc;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ZipCodesController: ControllerBase
{
    private readonly IZipCodeService _zipCodeService;

    public ZipCodesController(IZipCodeService zipCodeService)
    {
        _zipCodeService = zipCodeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IList<RecyclingEvent>), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetZipCodes()
    {
       return Ok(await _zipCodeService.FindAll());
    }
}