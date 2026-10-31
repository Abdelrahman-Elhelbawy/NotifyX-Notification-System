using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotifyX.Api.Configuration.RateLimiting;
using NotifyX.Api.Models.Common;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Services.Notifications;

namespace NotifyX.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class NotificationsController(
    INotificationService service) : ControllerBase
{
    private readonly INotificationService _service = service;

    /// <summary>
    /// Creates a notification for the users listed in the request.
    /// </summary>
    /// <param name="request">The notification details, including its title, body, type, priority, metadata, and recipient user IDs.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response containing the created notification, or a problem response if creation fails.</returns>
    [HttpPost]
    [Route("")]
    [ApiVersion(1.0)]
    [EnableRateLimiting(RateLimitPolicies.CreateNotification)]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        CancellationToken cancellationToken = default){
        var result = await _service.CreateAsync(request, idempotencyKey, cancellationToken);

        return result.Match<IActionResult>(
            notificationResponse => Ok(notificationResponse),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }

    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response containing the notifications, or a problem response if retrieval fails.</returns>
    [HttpGet]
    [Route("")]
    [ApiVersion(1.0)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(request, cancellationToken);

        return result.Match<IActionResult>(
            notificationResponse => Ok(notificationResponse),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }


    /// <summary>
    /// Retrieves a notification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response containing the notification, or a problem response if it is not found or retrieval fails.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    [ApiVersion(1.0)]
    public async Task<IActionResult> GetById(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAsync(
            id,
            cancellationToken);

        return result.Match<IActionResult>(
            notificationResponse => Ok(notificationResponse),
            error => Problem(
                statusCode: error.StatusCode,
                title: error.Code,
                detail: error.Description
            )
        );
    }

    /// <summary>
    /// Updates an existing notification.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to update.</param>
    /// <param name="request">The notification fields to update.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response when the notification is updated, or a problem response if the update fails.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    [ApiVersion(1.0)]
    [EnableRateLimiting(RateLimitPolicies.UpdateNotification)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);

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
    /// Deletes a notification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response when the notification is deleted, or a problem response if deletion fails.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ApiVersion(1.0)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);

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