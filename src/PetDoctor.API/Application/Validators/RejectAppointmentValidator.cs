using FluentValidation;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.Application.Validators
{
    public class RejectAppointmentValidator : AbstractValidator<RejectAppointment>
    {
        public RejectAppointmentValidator()
        {
            var maxReasonLength = 1000;
            RuleFor(p => p.Reason)
                .NotEmpty()
                .WithMessage($"{nameof(CancelAppointment.Reason)} is required")
                .MaximumLength(maxReasonLength)
                .WithMessage($"{nameof(CancelAppointment.Reason)} must not exceed {maxReasonLength} characters in length");
        }
    }
}
