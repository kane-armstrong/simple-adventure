using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Validators;

public class RejectAppointmentValidatorTests
{
    [Fact]
    public void Validation_should_fail_when_rejection_reason_is_null()
    {
        var request = new RejectAppointment
        {
            Reason = null!
        };

        var sut = new RejectAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }

    [Fact]
    public void Validation_should_fail_when_rejection_reason_is_empty()
    {
        var request = new RejectAppointment
        {
            Reason = string.Empty
        };

        var sut = new RejectAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }

    [Fact]
    public void Validation_should_fail_when_rejection_reason_is_longer_than_1000_characters()
    {
        var request = new RejectAppointment
        {
            Reason = new string('x', 1001)
        };

        var sut = new RejectAppointmentValidator();
        var result = sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(p => p.Reason);
    }
}