using MediatR;

namespace ContentManagementSystem.Core.Commands;

public class DeleteProgramCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
}