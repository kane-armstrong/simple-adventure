// This code was copied from the xunit samples repository here https://github.com/xunit/samples.xunit/tree/main/AssemblyFixtureExample
using System;

namespace PetDoctor.API.Tests.Functional.Infrastructure.XUnitExtensions
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyFixtureAttribute : Attribute
    {
        public Type FixtureType { get; }

        public AssemblyFixtureAttribute(Type fixtureType)
        {
            FixtureType = fixtureType;
        }
    }
}
