using SqlStreamStore;
using SqlStreamStore.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetDoctor.Infrastructure
{
    public static class StreamStoreExtensions
    {
        public static async Task<Queue<StreamMessage>> LoadEvents(this IStreamStore eventStream, Guid id, int batchSize = 100)
        {
            var streamId = new StreamId(id.ToString());

            var messages = new Queue<StreamMessage>();
            bool haveMoreEvents;
            var readFromVersion = 0;
            do
            {
                var page = await eventStream.ReadStreamForwards(streamId, readFromVersion, batchSize);
                foreach (var message in page.Messages)
                {
                    messages.Enqueue(message);
                }

                haveMoreEvents = !page.IsEnd;
                readFromVersion = page.NextStreamVersion;
            } while (haveMoreEvents);

            return messages;
        }
    }
}
