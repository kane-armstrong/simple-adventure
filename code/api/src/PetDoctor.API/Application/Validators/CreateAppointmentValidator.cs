using FluentValidation;
using PetDoctor.API.Application.Commands;
using System;
using Humanizer;

namespace PetDoctor.API.Application.Validators
{
    public class CreateAppointmentValidator : AbstractValidator<CreateAppointment>
    {
        public CreateAppointmentValidator()
        {
            const int maxReasonLength = 1000;
            RuleFor(p => p.ReasonForVisit)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.ReasonForVisit).Humanize()} is required")
                .MaximumLength(maxReasonLength)
                .WithMessage($"{nameof(CreateAppointment.ReasonForVisit).Humanize()} must not exceed {maxReasonLength} characters in length");

            RuleFor(p => p.DesiredDate)
                .Must(BeInTheFuture)
                .WithMessage($"{nameof(CreateAppointment.DesiredDate).Humanize()} must be in the future");

            const int maxOwnerFirstNameLength = 100;
            RuleFor(p => p.OwnerFirstName)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.OwnerFirstName).Humanize()} is required")
                .MaximumLength(maxOwnerFirstNameLength)
                .WithMessage($"{nameof(CreateAppointment.OwnerFirstName).Humanize()} must not exceed {maxOwnerFirstNameLength} characters in length");

            const int maxOwnerLastNameLength = 100;
            RuleFor(p => p.OwnerLastName)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.OwnerLastName).Humanize()} is required")
                .MaximumLength(maxOwnerLastNameLength)
                .WithMessage($"{nameof(CreateAppointment.OwnerLastName).Humanize()} must not exceed {maxOwnerLastNameLength} characters in length");

            const int maxOwnerEmailLength = 100;
            RuleFor(p => p.OwnerEmail)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.OwnerEmail).Humanize()} is required")
                .MaximumLength(maxOwnerEmailLength)
                .WithMessage($"{nameof(CreateAppointment.OwnerEmail).Humanize()} must not exceed {maxOwnerEmailLength} characters in length");

            const int maxOwnerPhoneLength = 25;
            RuleFor(p => p.OwnerPhone)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.OwnerPhone).Humanize()} is required")
                .MaximumLength(maxOwnerPhoneLength)
                .WithMessage($"{nameof(CreateAppointment.OwnerPhone).Humanize()} must not exceed {maxOwnerPhoneLength} characters in length");

            const int maxPetNameLength = 100;
            RuleFor(p => p.PetName)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.PetName).Humanize()} is required")
                .MaximumLength(maxPetNameLength)
                .WithMessage($"{nameof(CreateAppointment.PetName).Humanize()} must not exceed {maxPetNameLength} characters in length");

            RuleFor(p => p.PetDateOfBirth)
                .Must(BeInThePast)
                .WithMessage($"{nameof(CreateAppointment.PetDateOfBirth).Humanize()} must be in the past");

            const int maxPetBreedLength = 100;
            RuleFor(p => p.PetBreed)
                .NotEmpty()
                .WithMessage($"{nameof(CreateAppointment.PetBreed).Humanize()} is required")
                .MaximumLength(maxPetBreedLength)
                .WithMessage($"{nameof(CreateAppointment.PetBreed).Humanize()} must not exceed {maxPetBreedLength} characters in length");
        }

        private static bool BeInTheFuture(DateTimeOffset argument)
            // DateTimeOffset comparison converts compared values to UTC before doing the comparison, so this is safe
            => argument > DateTimeOffset.UtcNow;

        private static bool BeInThePast(DateTimeOffset argument)
            // DateTimeOffset comparison converts compared values to UTC before doing the comparison, so this is safe
            => argument < DateTimeOffset.UtcNow;
    }
}
