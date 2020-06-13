using System;
using MediatR;
using PetDoctor.API.Application.Models;

namespace PetDoctor.API.Application.Queries
{
    public class GetAppointmentById : IRequest<AppointmentView?>
    {
        public Guid Id { get; set; }
    }
}
