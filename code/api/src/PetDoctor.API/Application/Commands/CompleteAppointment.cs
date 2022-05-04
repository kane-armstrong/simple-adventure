namespace PetDoctor.API.Application.Commands;

public record CompleteAppointment
{
    internal Guid Id { get; set; }
}