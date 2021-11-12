using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure.Collections;
using System;

namespace PetDoctor.API.Application.Extensions;

public static class PaginatedListExtensions
{
    public static Page<T> ToPage<T>(this PaginatedList<T> @this) where T : class
    {
        if (@this is null)
        {
            throw new ArgumentNullException(nameof(@this));
        }
        return new Page<T>
        {
            PageSize = @this.PageSize,
            PageIndex = @this.PageIndex,
            Data = @this.ToList(),
            HasNextPage = @this.HasNextPage,
            HasPreviousPage = @this.HasPreviousPage,
            TotalCount = @this.TotalCount,
            TotalPages = @this.TotalPages
        };
    }
}