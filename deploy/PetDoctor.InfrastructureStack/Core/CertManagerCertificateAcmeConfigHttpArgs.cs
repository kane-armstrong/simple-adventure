using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerCertificateAcmeConfigHttpArgs : ResourceArgs
    {
        [Input("ingressClass", true)]
        public Input<string> IngressClass { get; set; }
    }
}