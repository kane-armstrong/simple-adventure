using Pulumi;

namespace PetDoctor.InfrastructureStack.Core
{
    public class AzureIdentityBindingSpecArgs
    {
        [Input("azureIdentity", true)]
        public string AzureIdentity { get; set; }
        [Input("selector", true)]
        public string Selector { get; set; }
    }
}