using InsightMed.Application.Modules.Notifications.Commands;
using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Application.Modules.Notifications.Models;
using InsightMed.Application.Modules.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    public async Task<ActionResult<GetAllNotificationsQueryResponse>> GetAllAsync(
        [FromQuery] NotificationFilter filter = NotificationFilter.All)
    {
        var response = await _sender.Send(new GetAllNotificationsQuery(filter));
        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAllAsync()
    {
        await _sender.Send(new DeleteAllNotificationsCommand());
        return NoContent();
    }
}