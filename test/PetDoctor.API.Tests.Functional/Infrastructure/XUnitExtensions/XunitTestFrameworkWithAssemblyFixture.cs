// This code was copied from the xunit samples repository here https://github.com/xunit/samples.xunit/tree/main/AssemblyFixtureExample
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PetDoctor.API.Tests.Functional.Infrastructure.XUnitExtensions
{
    public class XunitTestFrameworkWithAssemblyFixture : XunitTestFramework
    {
        public XunitTestFrameworkWithAssemblyFixture(IMessageSink messageSink)
            : base(messageSink)
        { }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new XunitTestFrameworkExecutorWithAssemblyFixture(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
