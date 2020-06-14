using FluentValidation;
using Humanizer;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.Application.Validators
{
    public class ConfirmAppointmentValidator : AbstractValidator<ConfirmAppointment>
    {
        public ConfirmAppointmentValidator()
        {
            RuleFor(p => p.AttendingVeterinarianId)
                .NotEmpty()
                .WithMessage($"{nameof(ConfirmAppointment.AttendingVeterinarianId).Humanize()} is required");
        }
    }
}
