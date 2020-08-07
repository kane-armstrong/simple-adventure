using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerClusterIssuerSpecArgs : ResourceArgs
    {
        [Input("acme", true)]
        public Input<CertManagerClusterIssuerAcmeArgs> Acme { get; set; }
    }
}