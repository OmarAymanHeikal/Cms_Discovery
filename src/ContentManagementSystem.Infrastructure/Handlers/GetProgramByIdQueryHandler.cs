using MediatR;
using ContentManagementSystem.Core.Queries;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Interfaces;

namespace ContentManagementSystem.Infrastructure.Handlers;

public class GetProgramByIdQueryHandler : IRequestHandler<GetProgramByIdQuery, Program?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProgramByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Program?> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var program = await _unitOfWork.Programs.GetByIdAsync(request.Id, cancellationToken);
        
        if (program != null)
        {
            // Increment view count for discovery
            await _unitOfWork.Programs.IncrementViewCountAsync(program.Id, cancellationToken);
        }

        return program;
    }
}