namespace Inventory.Core.DTOs.Requests;

public class ProductParameters
{
    // Filtering params
    public string? Category {get;set;}
    public string? SearchTerm{ get;set;}

    // Sorting
    public string? SortBy {get;set;}

    //Pagination
    public int PageNumber {get;set;} = 1;
    private int _pageSize = 5;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 50 ? 50 :value;
    }
}