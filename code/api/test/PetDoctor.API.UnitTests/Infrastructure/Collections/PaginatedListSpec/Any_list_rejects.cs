using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.API.UnitTests.Infrastructure.Collections.PaginatedListSpec;

public class Any_list_rejects
{
    [Fact]
    public void a_null_source()
    {
        var exception = Record.Exception(() => new PaginatedList<string>(null, 1, 1, 1));
        exception.Should().NotBeNull();
    }

    [Fact]
    public void a_non_positive_page_index()
    {
        var exception = Record.Exception(() => new PaginatedList<string>(new List<string>(), 0, 0, 1));
        exception.Should().NotBeNull();
    }

    [Fact]
    public void a_negative_page_size()
    {
        var exception = Record.Exception(() => new PaginatedList<string>(new List<string>(), 0, 1, -1));
        exception.Should().NotBeNull();
    }

    [Fact]
    public void a_negative_count()
    {
        var exception = Record.Exception(() => new PaginatedList<string>(new List<string>(), -1, 1, 0));
        exception.Should().NotBeNull();
    }
}