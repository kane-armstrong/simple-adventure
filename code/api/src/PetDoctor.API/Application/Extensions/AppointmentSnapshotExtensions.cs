using PetDoctor.API.Application.Models;
using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Extensions;

public static class AppointmentSnapshotExtensions
{
    public static AppointmentView ToAppointmentView(this AppointmentSnapshot appointment)
    {
        return new()
        {
            Id = appointment.Id,
            ScheduledOn = appointment.ScheduledOn,
            AttendingVeterinarianId = appointment.AttendingVeterinarianId,
            RejectionReason = appointment.RejectionReason,
            CancellationReason = appointment.CancellationReason,
            Owner = appointment.Owner,
            Pet = appointment.Pet,
            ReasonForVisit = appointment.ReasonForVisit,
            State = appointment.State.ToString()
        };
    }
}