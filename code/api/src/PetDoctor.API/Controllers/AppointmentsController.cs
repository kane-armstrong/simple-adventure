using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Extensions;
using PetDoctor.API.Application.Models;
using PetDoctor.API.Application.Queries;
using System;
using System.Threading.Tasks;

namespace PetDoctor.API.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Page<AppointmentView>>> ListAppointments([FromQuery] ListAppointments query)
    {
        var result = await _mediator.Send(query);
        return result.ToPage();
    }

    [HttpGet("{id}", Name = nameof(GetAppointmentById))]
    [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentView>> GetAppointmentById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetAppointmentById { Id = id });
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointment request)
    {
        var result = await _mediator.Send(request);
        const string route = nameof(GetAppointmentById);
        return CreatedAtRoute(route, new { id = result.ResourceId, version = "1" }, null);
    }

    [HttpPut("{id}/confirm")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmAppointment([FromRoute] Guid id, [FromBody] ConfirmAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectAppointment([FromRoute] Guid id, [FromBody] RejectAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/reschedule")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RescheduleAppointment([FromRoute] Guid id, [FromBody] RescheduleAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id, [FromBody] CancelAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/checkin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CheckinToAppointment([FromRoute] Guid id, [FromBody] CheckinToAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CompleteAppointment([FromRoute] Guid id, [FromBody] CompleteAppointment request)
    {
        request.Id = id;
        var result = await _mediator.Send(request);
        if (result is { ResourceFound: false })
            return NotFound();
        return NoContent();
    }
}