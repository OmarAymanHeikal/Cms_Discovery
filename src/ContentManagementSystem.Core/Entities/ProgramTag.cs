namespace ContentManagementSystem.Core.Entities;

public class ProgramTag
{
    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;
    
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}