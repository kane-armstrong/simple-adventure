using System;
using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Validators;

public class ConfirmAppointmentValidatorTests
{
    [Fact]
    public void Validation_should_fail_when_no_veterinaran_id_is_provided()
    {
        var request = new ConfirmAppointment
        {
            AttendingVeterinarianId = Guid.Empty
        };

        var sut = new ConfirmAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.AttendingVeterinarianId, request);
    }
}