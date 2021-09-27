using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.API.UnitTests.Infrastructure.Collections.QueryablePagingExtensions.PaginateSpec
{
    public class Any_set
    {
        [Fact]
        public void returns_an_empty_set_for_an_out_of_range_page()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(10);
            var query = set.AsQueryable().OrderBy(x => x).Paginate(3, 5);
            var sut = query.ToList();
            sut.Should().BeEmpty();
        }

        [Fact]
        public void returns_only_the_requested_set_for_an_in_range_page()
        {
            var set = new List<string>
            {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f"
            };
            var query = set.AsQueryable().OrderBy(x => x).Paginate(2, 2);
            var sut = query.ToList();
            sut.Count.Should().Be(2);
            sut.Any(x => x == "c").Should().BeTrue();
            sut.Any(x => x == "d").Should().BeTrue();
        }
    }
}
