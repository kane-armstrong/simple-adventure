using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.AadPodIdentity
{
    public class AzureIdentitySpecArgs : ResourceArgs
    {
        [Input("type", true)]
        public Input<int> Type { get; set; }
        [Input("resourceID", true)]
        public Input<string> ResourceId { get; set; }
        [Input("clientID", true)]
        public Input<string> ClientId { get; set; }
    }
}