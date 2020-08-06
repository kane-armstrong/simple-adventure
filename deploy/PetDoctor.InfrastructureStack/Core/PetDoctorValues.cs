namespace PetDoctor.InfrastructureStack.Core
{
    public class PetDoctorValues
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Namespace { get; set; }
        public string SecretName { get; set; }
        public string DockerConfigSecretName { get; set; }
        public ReplicaSetConfiguration AppointmentApi { get; set; }
    }
}