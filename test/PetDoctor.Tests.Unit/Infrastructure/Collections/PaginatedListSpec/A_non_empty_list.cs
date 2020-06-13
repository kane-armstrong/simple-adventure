using System.Linq;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.Tests.Unit.Infrastructure.Collections.PaginatedListSpec
{
    public class A_non_empty_list
    {
        [Fact]
        public void returns_the_original_source_when_tolist_invoked()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 2, 2);
            var source = sut.ToList();
            foreach (var point in set)
            {
                source.Contains(point).Should().BeTrue();
            }
        }

        [Fact]
        public void treats_1_as_the_first_page_index()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 1, 2).ToList().ToArray();
            var array = set.ToArray();
            array[0].Should().Be(sut[0]);
            array[1].Should().Be(sut[1]);
        }
    }
}
