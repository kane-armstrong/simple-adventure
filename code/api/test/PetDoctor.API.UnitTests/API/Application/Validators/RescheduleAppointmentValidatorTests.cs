using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Validators;

public class RescheduleAppointmentValidatorTests
{
    [Fact]
    public void Validation_should_fail_when_new_date_is_in_the_past()
    {
        var request = new RescheduleAppointment
        {
            NewDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var sut = new RescheduleAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.NewDate);
    }
}