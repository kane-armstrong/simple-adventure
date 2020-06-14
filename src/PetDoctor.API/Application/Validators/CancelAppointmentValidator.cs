﻿using FluentValidation;
using Humanizer;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.Application.Validators
{
    public class CancelAppointmentValidator : AbstractValidator<CancelAppointment>
    {
        public CancelAppointmentValidator()
        {
            const int maxReasonLength = 1000;

            RuleFor(p => p.Reason)
                .NotEmpty()
                .WithMessage($"{nameof(CancelAppointment.Reason).Humanize()} is required")
                .MaximumLength(maxReasonLength)
                .WithMessage($"{nameof(CancelAppointment.Reason).Humanize()} must not exceed {maxReasonLength} characters in length");
        }
    }
}
