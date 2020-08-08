using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerCertificateAcmeConfigArgs : ResourceArgs
    {
        [Input("http01", true)]
        public Input<CertManagerCertificateAcmeConfigHttpArgs> Http { get; set; }

        [Input("domains", true)]
        public InputList<string> Domains { get; set; }
    }
}