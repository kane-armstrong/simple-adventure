using Xunit;

namespace PetDoctor.API.IntegrationTests.Setup
{
    [CollectionDefinition(TestCollections.RealDatabaseTests, DisableParallelization = true)]
    public class RealDatabaseBackedTestCollection : ICollectionFixture<TestFixture>
    {
    }
}
