using InsightMed.Application.Notifications.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

// Temporary controller for testing queries and commands which are not supposed to expose endpoints
[ApiController]
[Route("api/[controller]")]
public sealed class TestCommandQueryController : ControllerBase
{
    private readonly ISender _sender;

    public TestCommandQueryController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpPost("createNotification")]
    public async Task<ActionResult> CreateNotificationAsync(NotificationTestInputModel input)
    {
        var command = new CreateNotificationCommand(input.Message, input.LabReportId);
        await _sender.Send(command);
        return NoContent();
    }
}

public class NotificationTestInputModel
{
    public string Message { get; set; } = string.Empty;
    public int LabReportId { get; set; }
}
