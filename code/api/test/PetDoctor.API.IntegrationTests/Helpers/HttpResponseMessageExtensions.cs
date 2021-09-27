using System;
using System.Net.Http;
using System.Threading.Tasks;
using PetDoctor.Infrastructure;

namespace PetDoctor.API.IntegrationTests.Helpers
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> GetPayload<T>(this HttpResponseMessage message)
        {
            var content = await message.Content.ReadAsStringAsync();
            try
            {
                return content.FromJson<T>();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Invalid payload");
            }
        }

        public static async Task ThrowWithBodyIfUnsuccessfulStatusCode(this HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
                return;

            var content = await message.Content.ReadAsStringAsync();
            throw new Exception($"Test failed - {message.StatusCode}: {content}");
        }
    }
}
