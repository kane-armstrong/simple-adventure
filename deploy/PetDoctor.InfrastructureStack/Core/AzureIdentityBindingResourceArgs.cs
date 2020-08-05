using Pulumi.Kubernetes.ApiExtensions;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentityBindingResourceArgs : CustomResourceArgs
    {
        public AzureIdentityBindingResourceArgs() : base("aadpodidentity.k8s.io/v1", "AzureIdentityBinding")
        {
        }

        public AzureIdentityBindingSpecArgs Spec { get; set; }
    }
}