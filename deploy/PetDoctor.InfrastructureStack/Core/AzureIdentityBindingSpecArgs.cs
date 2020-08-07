using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentityBindingSpecArgs : ResourceArgs
    {
        [Input("azureIdentity", true)]
        public Input<string> AzureIdentity { get; set; }
        [Input("selector", true)]
        public Input<string> Selector { get; set; }
    }
}