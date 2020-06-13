using System.Collections.Generic;

namespace PetDoctor.API.Application.Models
{
    public class Page<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
