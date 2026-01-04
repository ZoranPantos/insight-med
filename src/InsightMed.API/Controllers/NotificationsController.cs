using InsightMed.Application.Modules.Notifications.Commands;
using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Application.Modules.Notifications.Models;
using InsightMed.Application.Modules.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsightMed.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public NotificationsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllNotificationsQueryResponse>> GetAllAsync(
        [FromQuery] NotificationFilter filter = NotificationFilter.All)
    {
        var response = await _sender.Send(new GetAllNotificationsQuery(UserId, filter));
        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAllAsync()
    {
        await _sender.Send(new DeleteAllNotificationsCommand());
        return NoContent();
    }

    [HttpPut("seen")]
    public async Task<ActionResult> MarkAsSeenAsync([FromBody] List<int> ids)
    {
        await _sender.Send(new MarkNotificationsAsSeenCommand(UserId, ids));
        return NoContent();
    }
}