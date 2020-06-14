using FluentValidation;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.Application.Validators
{
    public class ConfirmAppointmentValidator : AbstractValidator<ConfirmAppointment>
    {
        public ConfirmAppointmentValidator()
        {
            RuleFor(p => p.AttendingVeterinarianId)
                .NotEmpty()
                .WithMessage($"{nameof(ConfirmAppointment.AttendingVeterinarianId)} is required");
        }
    }
}
