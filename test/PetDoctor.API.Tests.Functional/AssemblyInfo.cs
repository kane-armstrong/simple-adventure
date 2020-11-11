using PetDoctor.API.Tests.Functional.Infrastructure.XUnitExtensions;
using PetDoctor.API.Tests.Functional.Setup;
using Xunit;

[assembly: TestFramework("PetDoctor.API.Tests.Functional.Infrastructure.XUnitExtensions.XunitTestFrameworkWithAssemblyFixture", "PetDoctor.API.Tests.Functional")]
[assembly: AssemblyFixture(typeof(TestFixture))]
