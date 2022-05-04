using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Extensions;
using PetDoctor.API.Application.Models;
using PetDoctor.API.Application.Queries;

namespace PetDoctor.API.Controllers;

[Authorize("api")]
[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/appointments")]
public class AppointmentsController : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Page<AppointmentView>>> ListAppointments(
        [FromQuery] ListAppointments query,
        [FromServices] ListAppointmentsHandler handler)
    {
        var result = await handler.Handle(query, CancellationToken.None);
        return result.ToPage();
    }

    [HttpGet("{id}", Name = nameof(GetAppointmentById))]
    [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentView>> GetAppointmentById(
        [FromRoute] Guid id,
        [FromServices] GetAppointmentByIdHandler handler)
    {
        var result = await handler.Handle(new GetAppointmentById { Id = id }, CancellationToken.None);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointment request,
        [FromServices] CreateAppointmentHandler handler)
    {
        var result = await handler.Handle(request, CancellationToken.None);
        const string route = nameof(GetAppointmentById);
        return CreatedAtRoute(route, new { id = result.ResourceId, version = "1" }, null);
    }

    [HttpPut("{id}/confirm")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmAppointment(
        [FromRoute] Guid id,
        [FromBody] ConfirmAppointment request,
        [FromServices] ConfirmAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectAppointment(
        [FromRoute] Guid id,
        [FromBody] RejectAppointment request,
        [FromServices] RejectAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/reschedule")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RescheduleAppointment(
        [FromRoute] Guid id,
        [FromBody] RescheduleAppointment request,
        [FromServices] RescheduleAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelAppointment(
        [FromRoute] Guid id,
        [FromBody] CancelAppointment request,
        [FromServices] CancelAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/checkin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CheckinToAppointment(
        [FromRoute] Guid id,
        [FromBody] CheckinToAppointment request,
        [FromServices] CheckinToAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CompleteAppointment(
        [FromRoute] Guid id,
        [FromBody] CompleteAppointment request,
        [FromServices] CompleteAppointmentHandler handler)
    {
        request.Id = id;
        var result = await handler.Handle(request, CancellationToken.None);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }
}