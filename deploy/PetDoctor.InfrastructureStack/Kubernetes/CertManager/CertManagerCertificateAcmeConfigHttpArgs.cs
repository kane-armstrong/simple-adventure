using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerCertificateAcmeConfigHttpArgs : ResourceArgs
    {
        [Input("ingressClass", true)]
        public Input<string> IngressClass { get; set; }
    }
}