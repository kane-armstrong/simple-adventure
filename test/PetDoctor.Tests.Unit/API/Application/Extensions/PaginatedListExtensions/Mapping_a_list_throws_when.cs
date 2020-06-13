using System;
using PetDoctor.API.Application.Extensions;
using PetDoctor.Infrastructure.Collections;
using Xunit;

namespace PetDoctor.Tests.Unit.API.Application.Extensions.PaginatedListExtensions
{
    public class Mapping_a_list_throws_when
    {
        [Fact]
        public void the_input_list_is_null()
        {
            PaginatedList<string> sut = null;

            Assert.Throws<ArgumentNullException>(() => sut.ToPage());
        }
    }
}
