using System;
using MediatR;
using PetDoctor.API.Application.Models;

namespace PetDoctor.API.Application.Queries;

public record GetAppointmentById : IRequest<AppointmentView>
{
    public Guid Id { get; init; }
}