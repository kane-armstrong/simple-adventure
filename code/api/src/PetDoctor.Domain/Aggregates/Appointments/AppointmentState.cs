namespace PetDoctor.Domain.Aggregates.Appointments;

public enum AppointmentState
{
    Requested = 1,
    Confirmed = 2,
    Rejected = 3,
    Canceled = 4,
    CheckedIn = 5,
    Completed = 6,
    Paid = 7
}