using GestionInterventions.Application.Features.Notifications;
using GestionInterventions.Application.Features.Notifications.Commands.MarkNotificationsAsRead;
using GestionInterventions.Application.Features.Notifications.Queries.GetUnreadNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread(CancellationToken cancellationToken)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(identityUserId))
            return Unauthorized();

        var query = new GetUnreadNotificationsQuery(identityUserId);

        var notifications = await _mediator.Send(query, cancellationToken);

        return Ok(notifications);
    }

    [HttpPut("read")]
    public async Task<IActionResult> MarkAsRead([FromQuery] string type,CancellationToken cancellationToken)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(identityUserId))
            return Unauthorized();

        var command = new MarkNotificationsAsReadCommand(identityUserId, type);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}