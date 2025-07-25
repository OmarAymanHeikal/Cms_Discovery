namespace ContentManagementSystem.Core.Entities;

public class ProgramCategory
{
    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}