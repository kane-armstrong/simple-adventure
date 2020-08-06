using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentitySpecArgs
    {
        [Input("type", true)]
        public int Type { get; set; }
        [Input("resourceID", true)]
        public Output<string> ResourceId { get; set; }
        [Input("clientID", true)]
        public Output<string> ClientId { get; set; }
    }
}