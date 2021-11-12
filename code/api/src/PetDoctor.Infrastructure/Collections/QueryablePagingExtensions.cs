using System.Linq;

namespace PetDoctor.Infrastructure.Collections;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> Paginate<T>(this IOrderedQueryable<T> query, int pageIndex, int pageSize)
    {
        var entities = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        return entities;
    }
}