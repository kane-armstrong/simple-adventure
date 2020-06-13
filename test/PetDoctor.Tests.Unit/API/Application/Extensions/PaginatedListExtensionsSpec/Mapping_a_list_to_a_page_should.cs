using System.Collections.Immutable;
using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Extensions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.Tests.Unit.API.Application.Extensions.PaginatedListExtensionsSpec
{
    public class Mapping_a_list_to_a_page_should
    {
        [Fact]
        public void set_page_index_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.PageIndex.Should().Be(sut.PageIndex);
        }

        [Fact]
        public void set_page_size_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.PageSize.Should().Be(sut.PageSize);
        }

        [Fact]
        public void set_total_count_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.TotalCount.Should().Be(sut.TotalCount);
        }

        [Fact]
        public void set_total_pages_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.TotalPages.Should().Be(sut.TotalPages);
        }

        [Fact]
        public void set_has_previous_page_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.HasPreviousPage.Should().Be(sut.HasPreviousPage);
        }

        [Fact]
        public void set_has_next_page_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.HasNextPage.Should().Be(sut.HasNextPage);
        }

        [Fact]
        public void set_data_correctly()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<string>(5).ToImmutableList();
            var sut = new PaginatedList<string>(items, items.Count, 2, 3);

            var page = sut.ToPage();

            page.Data.Should().BeEquivalentTo(sut.ToList());
        }
    }
}
