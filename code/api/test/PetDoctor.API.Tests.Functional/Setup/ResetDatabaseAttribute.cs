using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace PetDoctor.API.Tests.Functional.Setup
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            ResetDatabases().GetAwaiter().GetResult();
        }

        private static async Task ResetDatabases()
        {
            await PetDoctorDatabaseCheckpoint.Reset();
        }
    }
}
