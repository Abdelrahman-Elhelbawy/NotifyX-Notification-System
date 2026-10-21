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

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

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

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.CreateAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok("Updated Successfully") : result.ToProblem();
    }
}