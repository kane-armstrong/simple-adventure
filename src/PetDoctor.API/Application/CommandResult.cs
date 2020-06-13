using System;

namespace PetDoctor.API.Application
{
    public struct CommandResult
    {
        public bool ResourceFound { get; }
        public Guid? ResourceId { get; }
        public bool? CommandSuccessful { get; }

        public CommandResult(bool resourceFound, bool commandSuccessful, Guid? resourceId)
        {
            ResourceFound = resourceFound;
            ResourceId = resourceId;
            CommandSuccessful = commandSuccessful;
        }
    }
}
