using PetDoctor.Infrastructure;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetDoctor.API.Tests.Functional.Helpers
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> GetPayload<T>(this HttpResponseMessage @this)
        {
            var content = await @this.Content.ReadAsStringAsync();
            try
            {
                return content.FromJson<T>();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Invalid payload");
            }
        }
    }
}
