using System.Reflection;
using AutoFixture.Kernel;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.Tests.Functional.Helpers
{
    public class CreateAppointmentBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            // Override default string generation to avoid binary data would be truncated errors

            if (request is ParameterInfo paramInfo
                && paramInfo.ParameterType == typeof(string)
                && paramInfo.Name == nameof(CreateAppointment.OwnerPhone))
            {
                return "212-000-0000";
            }

            var propInfo = request as PropertyInfo;
            if (propInfo != null
                && propInfo.PropertyType == typeof(string)
                && propInfo.Name == nameof(CreateAppointment.OwnerPhone))
            {
                return "212-000-0000";
            }

            return new NoSpecimen();
        }
    }
}