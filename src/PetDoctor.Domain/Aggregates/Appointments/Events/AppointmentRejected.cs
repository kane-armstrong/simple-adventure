﻿using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentRejected : DomainEvent
    {
        public Guid AppointmentId { get; }
        public string RejectionReason { get; }
        public AppointmentState State = AppointmentState.Rejected;

        public AppointmentRejected(Guid appointmentId, string rejectionReason)
        {
            AppointmentId = appointmentId;
            RejectionReason = rejectionReason;
        }
    }
}
