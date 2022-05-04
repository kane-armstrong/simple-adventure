namespace PetDoctor.Infrastructure.Collections;

public class PaginatedList<T>
{
    private readonly IReadOnlyList<T> _source;

    public PaginatedList(IReadOnlyList<T> source, int count, int pageIndex, int pageSize)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        PageIndex = pageIndex >= 1
            ? pageIndex
            : throw new ArgumentException($"{nameof(pageIndex)} must be greater than 0.");
        PageSize = pageSize >= 0 ? pageSize : throw new ArgumentException("Page size can not be negative.");
        TotalCount = count >= 0
            ? count
            : throw new ArgumentException("A page cannot have a negative number of items.");
        TotalPages = CalculateTotalPages(count, pageSize);
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    private int CalculateTotalPages(int count, int pageSize)
    {
        var result = (int)Math.Ceiling(count / (double)pageSize);
        return result < 0
            ? 0
            : result;
    }

    public IReadOnlyList<T> ToList()
    {
        return _source;
    }
}