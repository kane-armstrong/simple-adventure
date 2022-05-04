namespace PetDoctor.Domain.Aggregates.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment?> Find(Guid id, CancellationToken cancellationToken);
    Task Save(Appointment appointment, CancellationToken cancellationToken);
}