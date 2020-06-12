﻿using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentConfirmed : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentConfirmed(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
