using System.Collections.Generic;

namespace PetDoctor.API.Application.Models
{
    public record Page<T>
    {
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages { get; init; }
        public bool HasPreviousPage { get; init; }
        public bool HasNextPage { get; init; }
        public IReadOnlyList<T> Data { get; init; } = new List<T>().AsReadOnly();
    }
}
