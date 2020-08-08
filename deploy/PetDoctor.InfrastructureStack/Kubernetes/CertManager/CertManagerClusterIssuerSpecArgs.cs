using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerClusterIssuerSpecArgs : ResourceArgs
    {
        [Input("acme", true)]
        public Input<CertManagerClusterIssuerAcmeArgs> Acme { get; set; }
    }
}