﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Extensions;
using PetDoctor.API.Application.Models;
using PetDoctor.API.Application.Queries;
using System;
using System.Threading.Tasks;
using PetDoctor.API.Application;

namespace PetDoctor.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Page<AppointmentView>>> ListAppointments([FromQuery]ListAppointments query)
        {
            var result = await _mediator.Send(query);
            return result.ToPage();
        }

        [HttpGet("{id}", Name = nameof(GetAppointmentById))]
        [ProducesResponseType(typeof(Page<AppointmentView>), StatusCodes.Status200OK)]
        public async Task<ActionResult<AppointmentView>> GetAppointmentById([FromRoute]Guid id)
        {
            var result = await _mediator.Send(new GetAppointmentById { Id = id });
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAppointment([FromBody]CreateAppointment request)
        {
            ConfigureCommandContext(request);
            var result = await _mediator.Send(request);
            const string route = nameof(GetAppointmentById);
            return CreatedAtRoute(route, new { id = result.ResourceId }, null);
        }

        [HttpPut("{id}/confirm")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ConfirmAppointment([FromRoute]Guid id, [FromBody]ConfirmAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/reject")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectAppointment([FromRoute]Guid id, [FromBody]RejectAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/reschedule")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RescheduleAppointment([FromRoute]Guid id, [FromBody]RescheduleAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CancelAppointment([FromRoute]Guid id, [FromBody]CancelAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/checkin")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CheckinToAppointment([FromRoute]Guid id, [FromBody]CheckinToAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/complete")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CompleteAppointment([FromRoute]Guid id, [FromBody]CompleteAppointment request)
        {
            ConfigureCommandContext(request);
            request.Id = id;
            var result = await _mediator.Send(request);
            if (!result.ResourceFound)
                return NotFound();
            return NoContent();
        }

        private void ConfigureCommandContext(Command command)
        {
            // If I end up adding authentication then I could pass user info in through this (as opposed to relying on claims via IHttpContextAccessor)
            command.Context = new CommandContext();
        }
    }
}
