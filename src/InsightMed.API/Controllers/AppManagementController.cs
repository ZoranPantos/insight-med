using InsightMed.Application.AppManagement.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppManagementController : ControllerBase
{
    private readonly ISender _sender;

    public AppManagementController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet("[action]")]
    public async Task<ActionResult> SeedData()
    {
        try
        {
            await _sender.Send(new SeedDatabaseCommand());
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // TODO: Add endpoint for truncating data
}
