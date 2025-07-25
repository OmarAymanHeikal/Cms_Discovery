namespace ContentManagementSystem.Core.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<ProgramTag> ProgramTags { get; set; } = new List<ProgramTag>();
}