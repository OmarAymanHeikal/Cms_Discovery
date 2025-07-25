namespace ContentManagementSystem.Core.DTOs;

public class SearchCriteria
{
    public string? SearchTerm { get; set; }
    public int? Type { get; set; }
    public int? Language { get; set; }
    public int? Status { get; set; }
    public List<Guid> CategoryIds { get; set; } = new();
    public List<Guid> TagIds { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}