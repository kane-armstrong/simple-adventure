using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerClusterIssuerAcmeSecretArgs : ResourceArgs
    {
        [Input("name", true)]
        public Input<string> Name { get; set; }
    }
}