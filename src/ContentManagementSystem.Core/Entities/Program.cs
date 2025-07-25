using ContentManagementSystem.Core.Enums;

namespace ContentManagementSystem.Core.Entities;

public class Program : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime PublishedDate { get; set; }
    public ProgramType Type { get; set; }
    public Language Language { get; set; }
    public ProgramStatus Status { get; set; } = ProgramStatus.Draft;
    public int ViewCount { get; set; } = 0;
    public decimal Rating { get; set; } = 0;
    
    // Navigation properties
    public ICollection<ProgramCategory> ProgramCategories { get; set; } = new List<ProgramCategory>();
    public ICollection<ProgramTag> ProgramTags { get; set; } = new List<ProgramTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}