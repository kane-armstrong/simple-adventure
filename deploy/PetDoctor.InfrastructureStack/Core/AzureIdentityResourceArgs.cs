using Pulumi;
using Pulumi.Kubernetes.ApiExtensions;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentityResourceArgs : CustomResourceArgs
    {
        public AzureIdentityResourceArgs() : base("aadpodidentity.k8s.io/v1", "AzureIdentity")
        {
        }

        [Input("spec", true)]
        public Input<AzureIdentitySpecArgs> Spec { get; set; }
    }
}