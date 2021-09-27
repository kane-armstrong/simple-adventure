using MediatR;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetDoctor.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private const string AppointmentCanceled = "appointment_canceled";
        private const string AppointmentCompleted = "appointment_completed";
        private const string AppointmentConfirmed = "appointment_confirmed";
        private const string AppointmentCreated = "appointment_created";
        private const string AppointmentCheckedIn = "appointment_checkedin";
        private const string AppointmentRejected = "appointment_rejected";
        private const string AppointmentRescheduled = "appointment_rescheduled";

        private readonly IStreamStore _eventStream;
        private readonly IMediator _mediator;

        public AppointmentRepository(IStreamStore eventStream, IMediator mediator)
        {
            _eventStream = eventStream;
            _mediator = mediator;
        }

        private static readonly Dictionary<Type, string> EventTypeMap = new Dictionary<Type, string>
        {
            { typeof(AppointmentCanceled), AppointmentCanceled },
            { typeof(AppointmentCompleted), AppointmentCompleted },
            { typeof(AppointmentConfirmed), AppointmentConfirmed },
            { typeof(AppointmentCreated), AppointmentCreated },
            { typeof(AppointmentMembersCheckedIn), AppointmentCheckedIn },
            { typeof(AppointmentRejected), AppointmentRejected },
            { typeof(AppointmentRescheduled), AppointmentRescheduled },
        };

        public async Task<Appointment> Find(Guid id)
        {
            var messages = await _eventStream.LoadEvents(id);
            if (messages.Count == 0)
                return null;

            var createdEventPayload = await messages.Dequeue().GetJsonData();
            var createdEvent = createdEventPayload.FromJson<AppointmentCreated>();

            var appointment = new Appointment(createdEvent);

            var changes = await LoadReplayableEvents(messages);

            appointment.ReplayEvents(changes);

            return appointment;
        }

        public async Task Save(Appointment appointment)
        {
            foreach (var @event in appointment.PendingEvents)
            {
                if (!EventTypeMap.ContainsKey(@event.GetType()))
                {
                    throw new InvalidOperationException($"Unrecognized event type: {@event.GetType().FullName}");
                }
                await _eventStream.AppendToStream(
                    appointment.Id.ToString(),
                    ExpectedVersion.Any,
                    new NewStreamMessage(@event.Id, EventTypeMap[@event.GetType()], @event.ToJson()));
            }

            await DispatchEvents(appointment);
        }

        private static async Task<List<DomainEvent>> LoadReplayableEvents(IEnumerable<StreamMessage> messages)
        {
            var changes = new List<DomainEvent>();
            foreach (var @event in messages)
            {
                var jsonData = await @event.GetJsonData();
                switch (@event.Type)
                {
                    case AppointmentCanceled:
                        changes.Add(jsonData.FromJson<AppointmentCanceled>());
                        break;
                    case AppointmentCompleted:
                        changes.Add(jsonData.FromJson<AppointmentCompleted>());
                        break;
                    case AppointmentConfirmed:
                        changes.Add(jsonData.FromJson<AppointmentConfirmed>());
                        break;
                    case AppointmentCreated:
                        changes.Add(jsonData.FromJson<AppointmentCreated>());
                        break;
                    case AppointmentCheckedIn:
                        changes.Add(jsonData.FromJson<AppointmentMembersCheckedIn>());
                        break;
                    case AppointmentRejected:
                        changes.Add(jsonData.FromJson<AppointmentRejected>());
                        break;
                    case AppointmentRescheduled:
                        changes.Add(jsonData.FromJson<AppointmentRescheduled>());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(@event.Type), @event.Type, "Unsupported event type");
                }
            }

            return changes;
        }

        private async Task DispatchEvents(IEventSourcedEntity entity)
        {
            foreach (var @event in entity.PendingEvents)
            {
                await _mediator.Publish(@event);
            }
        }
    }
}