using FluentValidation;
using PetDoctor.API.Application.Commands;
using System;
using Humanizer;

namespace PetDoctor.API.Application.Validators;

public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointment>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(p => p.NewDate)
            .Must(BeInTheFuture)
            .WithMessage($"{nameof(RescheduleAppointment.NewDate).Humanize()} must be in the future");
    }

    private static bool BeInTheFuture(DateTimeOffset argument)
        // DateTimeOffset comparison converts compared values to UTC before doing the comparison, so this is safe
        => argument > DateTimeOffset.UtcNow;
}