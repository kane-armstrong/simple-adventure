using System;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointment : Command
    {
        public string PetName { get; set; }
        public DateTimeOffset PetDateOfBirth { get; set; }
        public string PetBreed { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public Guid? DesiredVerterinarianId { get; set; }
        public string ReasonForVisit { get; set; }
        public DateTimeOffset DesiredDate { get; set; }
    }
}