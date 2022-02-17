using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Validators;

// TODO remove all validation unit tests, move them to integration tests
public class CancelAppointmentValidatorTests
{
    [Fact]
    public void Validation_should_fail_when_cancellation_reason_is_null()
    {
        var request = new CancelAppointment
        {
            Reason = null
        };

        var sut = new CancelAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }

    [Fact]
    public void Validation_should_fail_when_cancellation_reason_is_empty()
    {
        var request = new CancelAppointment
        {
            Reason = string.Empty
        };

        var sut = new CancelAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }

    [Fact]
    public void Validation_should_fail_when_cancellation_reason_is_longer_than_1000_characters()
    {
        var request = new CancelAppointment
        {
            Reason = new string('x', 1001)
        };

        var sut = new CancelAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }
}