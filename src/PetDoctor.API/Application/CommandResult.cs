using System;

namespace PetDoctor.API.Application
{
    public struct CommandResult
    {
        public bool ResourceFound { get; init; }
        public Guid? ResourceId { get; init; }
    }
}
