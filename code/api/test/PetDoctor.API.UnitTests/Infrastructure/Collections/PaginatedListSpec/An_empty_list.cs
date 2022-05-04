using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.API.UnitTests.Infrastructure.Collections.PaginatedListSpec;

public class An_empty_list
{
    [Fact]
    public void returns_an_empty_list_when_tolist_invoked()
    {
        var sut = new PaginatedList<string>(new List<string>(), 0, 2, 2);
        var source = sut.ToList();
        source.Should().BeEmpty();
    }
}