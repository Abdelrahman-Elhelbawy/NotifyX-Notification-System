using Microsoft.AspNetCore.Mvc;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Services.Notifications;

namespace NotifyX.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(
    INotificationService service) : ControllerBase
{
    private readonly INotificationService _service = service;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}