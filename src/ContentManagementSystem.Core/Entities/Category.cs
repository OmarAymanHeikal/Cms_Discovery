namespace ContentManagementSystem.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<ProgramCategory> ProgramCategories { get; set; } = new List<ProgramCategory>();
}