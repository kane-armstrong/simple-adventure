using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerCertificateSpecArgs : ResourceArgs
    {
        [Input("secretName", true)]
        public Input<string> SecretName { get; set; }
        
        [Input("dnsNames", true)]
        public InputList<string> DnsNames { get; set; }
        
        [Input("acme", true)]
        public Input<CertManagerCertificateAcmeArgs> Acme { get; set; }
        
        [Input("issuerRef", true)]
        public Input<CertManagerCertificateIssuerRefArgs> IssuerRef { get; set; }
    }
}