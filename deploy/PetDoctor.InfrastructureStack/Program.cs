using Pulumi;
using System.Threading.Tasks;

namespace PetDoctor.InfrastructureStack
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<PetDoctorStack>();
    }
}
