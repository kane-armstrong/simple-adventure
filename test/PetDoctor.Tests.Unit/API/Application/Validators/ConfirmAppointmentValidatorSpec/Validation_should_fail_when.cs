using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using System;
using Xunit;

namespace PetDoctor.Tests.Unit.API.Application.Validators.ConfirmAppointmentValidatorSpec
{
    public class Validation_should_fail_when
    {
        [Fact]
        public void no_veterinaran_id_is_provided()
        {
            var request = new ConfirmAppointment
            {
                AttendingVeterinarianId = Guid.Empty
            };

            var sut = new ConfirmAppointmentValidator();

            sut.ShouldHaveValidationErrorFor(p => p.AttendingVeterinarianId, request);
        }
    }
}
