namespace PetDoctor.InfrastructureStack.Configuration
{
    public class PetDoctorClusterOptions
    {
        public string Domain { get; set; }
        public string Namespace { get; set; }
        public string CertificateIssuerAcmeEmail { get; set; }
        public ReplicaSetConfiguration AppointmentApi { get; set; }
    }
}