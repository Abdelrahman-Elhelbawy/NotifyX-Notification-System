using Microsoft.AspNetCore.Mvc;
using NotifyX.Api.Models.NotificationRecipient;
using NotifyX.Api.Services.NotificationRecipients;

namespace NotifyX.Api.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationRecipientsController(
    INotificationRecipientService service) : ControllerBase
{
    private readonly INotificationRecipientService _service = service;

    [HttpPut]
    [Route("{notificationId:guid}/read")]
    public async Task<IActionResult> UpdateReadAsync(
        [FromRoute] Guid notificationId,
        [FromBody] UpdateReadNotificationRecipientRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _service.ReadAsync(notificationId, request.userId, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }

    [HttpPut]
    [Route("{notificationId:guid}/unread")]
    public async Task<IActionResult> UpdateUnReadAsync(
        [FromRoute] Guid notificationId,
        [FromBody] UpdateReadNotificationRecipientRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _service.UnReadAsync(notificationId, request.userId, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }
}