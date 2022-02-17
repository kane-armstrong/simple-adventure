using FluentValidation;
using Humanizer;
using PetDoctor.API.Application.Queries;

namespace PetDoctor.API.Application.Validators
{
    public class ListAppointmentsValidator : AbstractValidator<ListAppointments>
    {
        public ListAppointmentsValidator()
        {
            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage($"{nameof(ListAppointments.PageIndex).Humanize()} must be a positive number.");

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage($"{nameof(ListAppointments.PageSize).Humanize()} must be a positive number.");
        }
    }
}
