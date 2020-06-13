﻿using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointmentHandler : IRequestHandler<CheckinToAppointment, CommandContext>
    {
        public Task<CommandContext> Handle(CheckinToAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
