namespace PetDoctor.Domain.Aggregates.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment> Find(Guid id);
    Task Save(Appointment appointment);
}