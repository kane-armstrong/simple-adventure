using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class CertManagerClusterIssuerAcmeSecretArgs : ResourceArgs
    {
        [Input("name", true)]
        public Input<string> Name { get; set; }
    }
}