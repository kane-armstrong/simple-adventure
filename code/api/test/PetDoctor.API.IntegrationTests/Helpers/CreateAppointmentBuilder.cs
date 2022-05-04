using AutoFixture.Kernel;
using PetDoctor.API.Application.Commands;
using System.Reflection;

namespace PetDoctor.API.IntegrationTests.Helpers;

public class CreateAppointmentBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is ParameterInfo paramInfo)
        {
            var result = Build(paramInfo.ParameterType, paramInfo.Name);
            if (result != null)
                return result;
        }

        var propInfo = request as PropertyInfo;
        if (propInfo != null)
        {
            var result = Build(propInfo.PropertyType, propInfo.Name);
            if (result != null)
                return result;
        }

        return new NoSpecimen();
    }

    private static object? Build(Type type, string? name)
    {
        if (type == typeof(string) && name == nameof(CreateAppointment.OwnerPhone))
        {
            return "212-000-0000";
        }

        if (type == typeof(DateTimeOffset) && name == nameof(CreateAppointment.PetDateOfBirth))
        {
            return DateTimeOffset.UtcNow.AddYears(-10);
        }

        if (type == typeof(DateTimeOffset) && name == nameof(CreateAppointment.DesiredDate))
        {
            return DateTimeOffset.UtcNow.AddDays(3);
        }

        return null;
    }
}