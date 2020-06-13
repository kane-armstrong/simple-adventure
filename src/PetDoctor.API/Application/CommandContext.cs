using System;

namespace PetDoctor.API.Application
{
    public class CommandContext
    {
        public bool ResourceFound { get; set; }
        public Guid ResourceId { get; set; }
    }
}
