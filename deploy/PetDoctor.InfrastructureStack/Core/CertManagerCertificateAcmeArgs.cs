using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerCertificateAcmeArgs : ResourceArgs
    {
        [Input("config", true)]
        public InputList<CertManagerCertificateAcmeConfigArgs> Config { get; set; }
    }
}