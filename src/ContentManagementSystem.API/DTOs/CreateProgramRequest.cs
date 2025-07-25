using System.ComponentModel.DataAnnotations;

namespace ContentManagementSystem.API.DTOs;

public class CreateProgramRequest
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Url]
    public string ThumbnailUrl { get; set; } = string.Empty;
    
    [Required]
    [Url]
    public string VideoUrl { get; set; } = string.Empty;
    
    [Required]
    public TimeSpan Duration { get; set; }
    
    [Required]
    public DateTime PublishedDate { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Type { get; set; }
    
    [Required]
    [Range(1, 4)]
    public int Language { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Status { get; set; }
    
    public List<Guid> CategoryIds { get; set; } = new();
    public List<Guid> TagIds { get; set; } = new();
}

public class UpdateProgramRequest : CreateProgramRequest
{
    [Required]
    public Guid Id { get; set; }
}

public class SearchProgramsRequest
{
    public string? SearchTerm { get; set; }
    public int? Type { get; set; }
    public int? Language { get; set; }
    public int? Status { get; set; }
    public List<Guid> CategoryIds { get; set; } = new();
    public List<Guid> TagIds { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}