using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using NotifyX.Api.Models.NotificationRecipient;
using NotifyX.Api.Services.NotificationRecipients;

namespace NotifyX.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/notifications")]
public class NotificationRecipientsController(
    INotificationRecipientService service) : ControllerBase
{
    private readonly INotificationRecipientService _service = service;

    /// <summary>
    /// Marks a notification as read for the specified user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as read.</param>
    /// <param name="request">The request containing the user ID.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 204 response if successful, or a problem response if the operation fails.</returns>
    [HttpPut]
    [Route("{notificationId:guid}/read")]
    [ApiVersion(1.0)]
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

    /// <summary>
    /// Marks a notification as unread for the specified user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as unread.</param>
    /// <param name="request">The request containing the user ID.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 204 response if successful, or a problem response if the operation fails.</returns>
    [HttpPut]
    [Route("{notificationId:guid}/unread")]
    [ApiVersion(1.0)]
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

    /// <summary>
    /// Marks a notification as unread for the specified user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as unread.</param>
    /// <param name="request">The request containing the user ID.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 204 response if successful, or a problem response if the operation fails.</returns>
    [HttpGet]
    [Route("{notificationId:guid}/{userId}/unread-count")]
    [ApiVersion(1.0)]
    public async Task<IActionResult> UpdateUnReadCountAsync(
        [FromRoute] Guid notificationId,
        [FromRoute] string userId ,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _service.UnReadCountAsync(notificationId, userId, cancellationToken);

        return result.Match<IActionResult>(
            count => Ok(new
            {
                notificationId,
                unreadCount = count
            }),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }

}