using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentitySpecArgs
    {
        [Input("type", true)]
        public int Type { get; set; }
        [Input("resourceID", true)]
        public string ResourceId { get; set; }
        [Input("clientID", true)]
        public string ClientId { get; set; }
    }
}