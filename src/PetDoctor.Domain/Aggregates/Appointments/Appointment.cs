using System;
using PetDoctor.Domain.Aggregates.Appointments.Events;

namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class Appointment : EventSourcedEntity
    {
        public Pet Pet { get; }
        public Owner Owner { get; }
        public Guid? AttendingVeterinarianId { get; private set; }
        public string ReasonForVisit { get;  }
        public DateTimeOffset ScheduledOn { get; private set; }
        public DateTimeOffset? CompletedOn { get; private set; }
        public AppointmentState State { get; private set; }

        public Appointment(AppointmentCreated @event)
        {
            Pet = @event.Data.Pet;
            Owner = @event.Data.Owner;
            AttendingVeterinarianId = @event.Data.AttendingVeterinarianId;
            ReasonForVisit = @event.Data.ReasonForVisit;
            ScheduledOn = @event.Data.ScheduledOn;
            State = @event.Data.State;
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

        public void Reject()
        {
            State = AppointmentState.Rejected;
            AppendEvent(new AppointmentRejected(Id, CreateMemento()));
        }

        public void Reschedule(DateTimeOffset date)
        {
            State = AppointmentState.Requested;
            AppendEvent(new AppointmentRescheduled(Id, CreateMemento()));
        }

        public void Cancel()
        {
            State = AppointmentState.Canceled;
            AppendEvent(new AppointmentCanceled(Id, CreateMemento()));
        }

        public void CheckIn()
        {
            State = AppointmentState.CheckedIn;
            AppendEvent(new AppointmentMembersCheckedIn(Id, CreateMemento()));
        }

        public void Complete()
        {
            State = AppointmentState.Completed;
            AppendEvent(new AppointmentCompleted(Id, CreateMemento()));
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
                ScheduledOn = ScheduledOn
            };
        }
    }
}
