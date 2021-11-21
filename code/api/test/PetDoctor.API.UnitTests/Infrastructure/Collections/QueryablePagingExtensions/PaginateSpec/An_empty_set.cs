using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.API.UnitTests.Infrastructure.Collections.QueryablePagingExtensions.PaginateSpec;

public class An_empty_set
{
    [Fact]
    public void returns_an_empty_set()
    {
        var query = new List<string>().AsQueryable().OrderBy(x => x).Paginate(1, 5);
        var sut = query.ToList();
        sut.Should().BeEmpty();
    }
}