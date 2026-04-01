namespace Inventory.Core.DTOs.Responses;

public class PagedResponse<T> : ApiResponse<T>
{
    public int CurrentPage {get;set;}
    public int TotalPages {get;set;}
    public int PageSize {get;set;}
    public int TotalCount { get;set;}

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedResponse(T data, int count, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count/(double)pageSize);
        Success = true;
    }
}