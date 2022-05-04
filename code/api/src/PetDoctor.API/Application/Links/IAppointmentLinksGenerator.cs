namespace PetDoctor.API.Application.Links;

public interface IAppointmentLinksGenerator
{
    string GenerateSelfLink(Guid appointmentId);
}