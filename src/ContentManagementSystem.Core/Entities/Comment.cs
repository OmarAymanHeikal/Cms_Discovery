namespace ContentManagementSystem.Core.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    
    // Navigation properties
    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;
}