using System.Linq;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.Tests.Unit.Infrastructure.Collections.PaginatedListSpec
{
    public class A_new_list
    {
        [Fact]
        public void sets_page_index_correctly()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(5).ToList();
            const int pageIndex = 1;
            var sut = new PaginatedList<string>(set, set.Count, pageIndex, 5);
            sut.PageIndex.Should().Be(pageIndex);
        }

        [Fact]
        public void sets_page_size_correctly()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(5).ToList();
            const int pageSize = 5;
            var sut = new PaginatedList<string>(set, set.Count, 1, pageSize);
            sut.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void sets_total_count_correctly()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(5).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 1, 5);
            sut.TotalCount.Should().Be(set.Count);
        }

        [Fact]
        public void sets_total_pages_correctly()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(5).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 1, 2);
            sut.TotalPages.Should().Be(3);
        }

        [Fact]
        public void sets_has_previous_page_correctly_when_there_is_no_previous_page()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 1, 2);
            sut.HasPreviousPage.Should().Be(false);
        }

        [Fact]
        public void sets_has_previous_page_correctly_when_there_is_a_previous_page()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 2, 2);
            sut.HasPreviousPage.Should().Be(true);
        }

        [Fact]
        public void sets_has_next_page_correctly_when_there_is_a_next_page()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 1, 2);
            sut.HasNextPage.Should().Be(true);
        }

        [Fact]
        public void sets_has_next_page_correctly_when_there_is_no_next_page()
        {
            var fixture = new Fixture();
            var set = fixture.CreateMany<string>(4).ToList();
            var sut = new PaginatedList<string>(set, set.Count, 2, 2);
            sut.HasNextPage.Should().Be(false);
        }
    }
}
