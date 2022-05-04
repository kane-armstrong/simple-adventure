namespace PetDoctor.API.Application.Queries;

public record GetAppointmentById
{
    public Guid Id { get; init; }
}