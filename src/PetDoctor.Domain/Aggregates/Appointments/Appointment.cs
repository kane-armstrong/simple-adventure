using System;

namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class Appointment
    {
        public Guid Id { get;  }
        public Pet Pet { get; }
        public Owner Owner { get; }
        public Guid? AttendingVeterinarianId { get;  }
        public string ReasonForVisit { get;  }
        public DateTimeOffset ScheduledOn { get;  }
        public DateTimeOffset CompletedOn { get;  }
        public AppointmentState State { get;  }

        public Appointment(
            Pet pet,
            Owner owner,
            string reasonForVisit,
            DateTimeOffset scheduledOn,
            DateTimeOffset completedOn,
            AppointmentState state)
        {
            Id = Guid.NewGuid();
            Pet = pet;
            Owner = owner;
            ReasonForVisit = reasonForVisit;
            ScheduledOn = scheduledOn;
            CompletedOn = completedOn;
            State = state;
        }

        public Appointment(
            Pet pet,
            Owner owner,
            Guid? attendingVeterinarianId,
            string reasonForVisit,
            DateTimeOffset scheduledOn,
            DateTimeOffset completedOn,
            AppointmentState state)
        {
            Id = Guid.NewGuid();
            Pet = pet;
            Owner = owner;
            AttendingVeterinarianId = attendingVeterinarianId;
            ReasonForVisit = reasonForVisit;
            ScheduledOn = scheduledOn;
            CompletedOn = completedOn;
            State = state;
        }
    }
}
