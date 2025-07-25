using MediatR;
using ContentManagementSystem.Core.Entities;

namespace ContentManagementSystem.Core.Commands;

public class UpdateProgramCommand : IRequest<Program>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime PublishedDate { get; set; }
    public int Type { get; set; }
    public int Language { get; set; }
    public int Status { get; set; }
    public List<Guid> CategoryIds { get; set; } = new();
    public List<Guid> TagIds { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
}