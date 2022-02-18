using Pulumi;
using Pulumi.Kubernetes.ApiExtensions;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerCertificateResourceArgs : CustomResourceArgs
    {
        public CertManagerCertificateResourceArgs() : base("certmanager.k8s.io/v1alpha1", "Certificate")
        {
        }

        [Input("spec", true)]
        public Input<CertManagerCertificateSpecArgs> Spec { get; set; }
    }
}