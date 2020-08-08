using Pulumi;

namespace PetDoctor.InfrastructureStack.Configuration
{
    public class ReplicaSetConfiguration
    {
        public int Port { get; set; }
        public int ReplicaCount { get; set; }
        public string AadPodIdentityName { get; set; }
        public string AadPodIdentityBindingName { get; set; }
        public string AadPodIdentitySelector { get; set; }
        public string DeploymentName { get; set; }
        public string IngressName { get; set; }
        public string ServiceName { get; set; }
        public Output<string> Image { get; set; }
        public ResourceLimit Memory { get; set; }
        public ResourceLimit Cpu { get; set; }
        public string SecretName { get; set; }
    }
}