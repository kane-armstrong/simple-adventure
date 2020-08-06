namespace PetDoctor.InfrastructureStack.Core
{
    public class PetDoctorValues
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Namespace { get; set; }
        public string SecretName { get; set; }
        public string BuildVersion { get; set; }
        public string Registry { get; set; }
        public ReplicaSetConfiguration AppointmentApi { get; set; }
    }
}