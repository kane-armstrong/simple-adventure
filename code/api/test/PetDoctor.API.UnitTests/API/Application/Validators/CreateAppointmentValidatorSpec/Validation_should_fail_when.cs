using System;
using FluentValidation.TestHelper;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Validators;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Validators.CreateAppointmentValidatorSpec;

public class Validation_should_fail_when
{
    [Fact]
    public void reason_for_visit_is_null()
    {
        var request = new CreateAppointment
        {
            ReasonForVisit = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.ReasonForVisit, request);
    }

    [Fact]
    public void reason_for_visit_is_empty()
    {
        var request = new CreateAppointment
        {
            ReasonForVisit = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.ReasonForVisit, request);
    }

    [Fact]
    public void reason_for_visit_is_longer_than_1000_characters()
    {
        var request = new CreateAppointment
        {
            ReasonForVisit = new string('x', 1001)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.ReasonForVisit, request);
    }

    [Fact]
    public void desired_date_is_in_the_past()
    {
        var request = new CreateAppointment
        {
            DesiredDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.DesiredDate, request);
    }

    [Fact]
    public void owner_first_name_is_null()
    {
        var request = new CreateAppointment
        {
            OwnerFirstName = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerFirstName, request);
    }

    [Fact]
    public void owner_first_name_is_empty()
    {
        var request = new CreateAppointment
        {
            OwnerFirstName = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerFirstName, request);
    }

    [Fact]
    public void owner_first_name_is_longer_than_100_characters()
    {
        var request = new CreateAppointment
        {
            OwnerFirstName = new string('x', 101)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerFirstName, request);
    }

    [Fact]
    public void owner_last_name_is_null()
    {
        var request = new CreateAppointment
        {
            OwnerLastName = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerLastName, request);
    }

    [Fact]
    public void owner_last_name_is_empty()
    {
        var request = new CreateAppointment
        {
            OwnerLastName = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerLastName, request);
    }

    [Fact]
    public void owner_last_name_is_longer_than_100_characters()
    {
        var request = new CreateAppointment
        {
            OwnerLastName = new string('x', 101)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerLastName, request);
    }
        
    [Fact]
    public void owner_email_is_null()
    {
        var request = new CreateAppointment
        {
            OwnerEmail = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerEmail, request);
    }

    [Fact]
    public void owner_email_is_empty()
    {
        var request = new CreateAppointment
        {
            OwnerEmail = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerEmail, request);
    }

    [Fact]
    public void owner_email_is_longer_than_100_characters()
    {
        var request = new CreateAppointment
        {
            OwnerEmail = new string('x', 101)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerEmail, request);
    }

    [Fact]
    public void owner_phone_is_null()
    {
        var request = new CreateAppointment
        {
            OwnerPhone = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerPhone, request);
    }

    [Fact]
    public void owner_phone_is_empty()
    {
        var request = new CreateAppointment
        {
            OwnerPhone = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerPhone, request);
    }

    [Fact]
    public void owner_phone_is_longer_than_25_characters()
    {
        var request = new CreateAppointment
        {
            OwnerPhone = new string('x', 26)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.OwnerPhone, request);
    }

    [Fact]
    public void pet_name_is_null()
    {
        var request = new CreateAppointment
        {
            PetName = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetName, request);
    }

    [Fact]
    public void pet_name_is_empty()
    {
        var request = new CreateAppointment
        {
            PetName = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetName, request);
    }

    [Fact]
    public void pet_name_is_longer_than_100_characters()
    {
        var request = new CreateAppointment
        {
            PetName = new string('x', 101)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetName, request);
    }
        
    [Fact]
    public void pet_breed_is_null()
    {
        var request = new CreateAppointment
        {
            PetBreed = null
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetBreed, request);
    }

    [Fact]
    public void pet_breed_is_empty()
    {
        var request = new CreateAppointment
        {
            PetBreed = string.Empty
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetBreed, request);
    }

    [Fact]
    public void pet_breed_is_longer_than_100_characters()
    {
        var request = new CreateAppointment
        {
            PetBreed = new string('x', 101)
        };

        var sut = new CreateAppointmentValidator();

        sut.ShouldHaveValidationErrorFor(p => p.PetBreed, request);
    }
}