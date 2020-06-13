using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Collections.Generic;

namespace PetDoctor.Domain.Aggregates.Appointments
{
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

        public void Apply(AppointmentConfirmed @event)
        {
            AttendingVeterinarianId = @event.AttendingVeterinarianId;
            State = AppointmentState.Confirmed;
        }

        public void Apply(AppointmentRejected @event)
        {
            RejectionReason = @event.RejectionReason;
            State = AppointmentState.Rejected;
        }

        public void Apply(AppointmentRescheduled @event)
        {
            ScheduledOn = @event.Date;
            State = AppointmentState.Requested;
        }

        public void Apply(AppointmentCanceled @event)
        {
            CancellationReason = @event.CancellationReason;
            State = AppointmentState.Canceled;
        }

        public void Apply(AppointmentMembersCheckedIn @event)
        {
            State = AppointmentState.CheckedIn;
        }

        public void Apply(AppointmentCompleted @event)
        {
            State = AppointmentState.Completed;
        }

        public void ReplayEvents(IReadOnlyCollection<DomainEvent> events)
        {
            foreach (var domainEvent in events)
            {
                switch (domainEvent)
                {
                    case AppointmentConfirmed ac:
                        Apply(ac);
                        break;
                    case AppointmentRejected ar:
                        Apply(ar);
                        break;
                    case AppointmentRescheduled ars:
                        Apply(ars);
                        break;
                    case AppointmentCanceled acl:
                        Apply(acl);
                        break;
                    case AppointmentMembersCheckedIn amc:
                        Apply(amc);
                        break;
                    case AppointmentCompleted acm:
                        Apply(acm);
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
                ScheduledOn = ScheduledOn
            };
        }
    }
}
