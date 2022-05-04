using PetDoctor.Domain.Aggregates.Appointments.Events;

namespace PetDoctor.Domain.Aggregates.Appointments;

public class Appointment : EventSourcedEntity
{
    public Pet Pet { get; }
    public Owner Owner { get; }
    public Guid? AttendingVeterinarianId { get; private set; }
    public string ReasonForVisit { get; }
    public string? RejectionReason { get; set; }
    public string? CancellationReason { get; set; }
    public DateTimeOffset ScheduledOn { get; private set; }
    public AppointmentState State { get; private set; }

    public Appointment(AppointmentCreated @event)
    {
        var (appointmentId, appointmentMemento) = @event;

        Id = appointmentId;
        Pet = appointmentMemento.Pet;
        Owner = appointmentMemento.Owner;
        AttendingVeterinarianId = appointmentMemento.AttendingVeterinarianId;
        ReasonForVisit = appointmentMemento.ReasonForVisit;
        ScheduledOn = appointmentMemento.ScheduledOn;
        State = appointmentMemento.State;
    }

    public Appointment(
        Pet pet,
        Owner owner,
        string reasonForVisit,
        DateTimeOffset scheduledOn)
    {
        Id = Guid.NewGuid();
        Pet = pet;
        Owner = owner;
        ReasonForVisit = reasonForVisit;
        ScheduledOn = scheduledOn;
        State = AppointmentState.Requested;

        AppendEvent(new AppointmentCreated(Id, CreateMemento()));
    }

    public Appointment(
        Pet pet,
        Owner owner,
        Guid? attendingVeterinarianId,
        string reasonForVisit,
        DateTimeOffset scheduledOn)
    {
        Id = Guid.NewGuid();
        Pet = pet;
        Owner = owner;
        AttendingVeterinarianId = attendingVeterinarianId;
        ReasonForVisit = reasonForVisit;
        ScheduledOn = scheduledOn;
        State = AppointmentState.Requested;

        AppendEvent(new AppointmentCreated(Id, CreateMemento()));
    }

    public void Confirm(Guid attendingVeterinarianId)
    {
        State = AppointmentState.Confirmed;
        AttendingVeterinarianId = attendingVeterinarianId;
        AppendEvent(new AppointmentConfirmed(Id, attendingVeterinarianId));
    }

    public void Reject(string reason)
    {
        State = AppointmentState.Rejected;
        RejectionReason = reason;
        AppendEvent(new AppointmentRejected(Id, reason));
    }

    public void Reschedule(DateTimeOffset date)
    {
        State = AppointmentState.Requested;
        ScheduledOn = date;
        AppendEvent(new AppointmentRescheduled(Id, date));
    }

    public void Cancel(string reason)
    {
        State = AppointmentState.Canceled;
        CancellationReason = reason;
        AppendEvent(new AppointmentCanceled(Id, reason));
    }

    public void CheckIn()
    {
        State = AppointmentState.CheckedIn;
        AppendEvent(new AppointmentMembersCheckedIn(Id));
    }

    public void Complete()
    {
        State = AppointmentState.Completed;
        AppendEvent(new AppointmentCompleted(Id));
    }

    public void ReplayEvents(IReadOnlyCollection<DomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            switch (domainEvent)
            {
                case AppointmentConfirmed ac:
                    When(ac);
                    break;
                case AppointmentRejected ar:
                    When(ar);
                    break;
                case AppointmentRescheduled ars:
                    When(ars);
                    break;
                case AppointmentCanceled acl:
                    When(acl);
                    break;
                case AppointmentMembersCheckedIn amc:
                    When(amc);
                    break;
                case AppointmentCompleted acm:
                    When(acm);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(domainEvent),
                        domainEvent.GetType().Name,
                        "This event is not replayable");
            }
        }
    }

    public AppointmentMemento CreateMemento()
    {
        return new AppointmentMemento
        {
            State = State,
            AttendingVeterinarianId = AttendingVeterinarianId,
            Owner = Owner,
            Pet = Pet,
            ReasonForVisit = ReasonForVisit,
            ScheduledOn = ScheduledOn,
            RejectionReason = RejectionReason,
            CancellationReason = CancellationReason
        };
    }

    private void When(AppointmentConfirmed @event)
    {
        AttendingVeterinarianId = @event.AttendingVeterinarianId;
        State = @event.State;
    }

    private void When(AppointmentRejected @event)
    {
        RejectionReason = @event.RejectionReason;
        State = @event.State;
    }

    private void When(AppointmentRescheduled @event)
    {
        ScheduledOn = @event.Date;
        State = @event.State;
    }

    private void When(AppointmentCanceled @event)
    {
        CancellationReason = @event.CancellationReason;
        State = @event.State;
    }

    private void When(AppointmentMembersCheckedIn @event)
    {
        State = @event.State;
    }

    private void When(AppointmentCompleted @event)
    {
        State = @event.State;
    }
}