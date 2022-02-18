using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerCertificateAcmeArgs : ResourceArgs
    {
        [Input("config", true)]
        public InputList<CertManagerCertificateAcmeConfigArgs> Config { get; set; }
    }
}