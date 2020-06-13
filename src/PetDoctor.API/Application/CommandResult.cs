using System;

namespace PetDoctor.API.Application
{
    public struct CommandResult
    {
        public bool ResourceFound { get; }
        public Guid? ResourceId { get; }

        public CommandResult(bool resourceFound, Guid? resourceId)
        {
            ResourceFound = resourceFound;
            ResourceId = resourceId;
        }
    }
}
