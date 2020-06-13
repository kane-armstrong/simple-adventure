using Xunit;

namespace PetDoctor.API.Tests.Functional.Setup
{
    [CollectionDefinition(TestCollections.RealDatabaseTests, DisableParallelization = true)]
    public class RealDatabaseBackedTestCollection : ICollectionFixture<TestFixture>
    {
    }
}
