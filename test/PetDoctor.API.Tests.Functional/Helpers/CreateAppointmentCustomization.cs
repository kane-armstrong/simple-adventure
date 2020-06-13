using AutoFixture;

namespace PetDoctor.API.Tests.Functional.Helpers
{
    public class CreateAppointmentCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new CreateAppointmentBuilder());
        }
    }
}