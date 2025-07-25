using MediatR;
using ContentManagementSystem.Core.Entities;

namespace ContentManagementSystem.Core.Queries;

public class GetProgramByIdQuery : IRequest<Program?>
{
    public Guid Id { get; set; }
}