using InsightMed.Application.Notifications.Models;
using InsightMed.Application.Notifications.Queries;
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
    public async Task<ActionResult<GetAllNotificationsQueryResponse>> GetAllAsync()
    {
        var response = await _sender.Send(new GetAllNotificationsQuery());
        return Ok(response);
    }
}
