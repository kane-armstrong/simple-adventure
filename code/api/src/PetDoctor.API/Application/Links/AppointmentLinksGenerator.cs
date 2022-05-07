using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using PetDoctor.API.Controllers;

namespace PetDoctor.API.Application.Links
{
    public class AppointmentLinksGenerator : IAppointmentLinksGenerator
    {
        private readonly IUrlHelper _urlHelper;

        public AppointmentLinksGenerator(
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext!);
        }

        public string GenerateSelfLink(Guid appointmentId)
        {
            return _urlHelper.Link(nameof(AppointmentsController.GetAppointmentById), new { id = appointmentId })!;
        }
    }
}
