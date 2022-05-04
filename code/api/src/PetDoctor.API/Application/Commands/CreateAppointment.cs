namespace PetDoctor.API.Application.Commands;

public record CreateAppointment
{
    public string PetName { get; init; } = string.Empty;
    public DateTimeOffset PetDateOfBirth { get; init; }
    public string PetBreed { get; init; } = string.Empty;
    public string OwnerFirstName { get; init; } = string.Empty;
    public string OwnerLastName { get; init; } = string.Empty;
    public string OwnerPhone { get; init; } = string.Empty;
    public string OwnerEmail { get; init; } = string.Empty;
    public Guid? DesiredVerterinarianId { get; init; }
    public string ReasonForVisit { get; init; } = string.Empty;
    public DateTimeOffset DesiredDate { get; init; }
}