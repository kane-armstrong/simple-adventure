using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using System;
using Xunit;

namespace PetDoctor.Tests.Unit.API.Application.Validators.RescheduleAppointmentValidatorSpec
{
    public class Validation_should_fail_when
    {
        [Fact]
        public void new_date_is_in_the_past()
        {
            var request = new RescheduleAppointment
            {
                NewDate = DateTimeOffset.UtcNow.AddDays(-1)
            };

            var sut = new RescheduleAppointmentValidator();

            sut.ShouldHaveValidationErrorFor(p => p.NewDate, request);
        }
    }
}
