using Microsoft.AspNetCore.Mvc;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Services.Notifications;

namespace NotifyX.Api.Controllers;

[Route("api/[controller]")]
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
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response containing the notifications, or a problem response if retrieval fails.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Retrieves a notification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response containing the notification, or a problem response if it is not found or retrieval fails.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAsync(
            id,
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing notification.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to update.</param>
    /// <param name="request">The notification fields to update.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response when the notification is updated, or a problem response if the update fails.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Deletes a notification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>An HTTP 200 response when the notification is deleted, or a problem response if deletion fails.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}