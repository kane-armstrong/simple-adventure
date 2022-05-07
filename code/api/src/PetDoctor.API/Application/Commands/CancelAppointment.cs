namespace PetDoctor.API.Application.Commands;

public record CancelAppointment
{
    internal Guid Id { get; set; }
    public string Reason { get; init; } = string.Empty;
}