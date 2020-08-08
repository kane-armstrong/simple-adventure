using Pulumi;

namespace PetDoctor.InfrastructureStack.Kubernetes.CertManager
{
    public class CertManagerClusterIssuerAcmeArgs : ResourceArgs
    {
        [Input("server", true)]
        public Input<string> Server { get; set; }
        [Input("email", true)]
        public Input<string> Email { get; set; }
        [Input("privateKeySecretRef", true)]
        public Input<CertManagerClusterIssuerAcmeSecretArgs> PrivateKeySecretRef { get; set; }
    }
}