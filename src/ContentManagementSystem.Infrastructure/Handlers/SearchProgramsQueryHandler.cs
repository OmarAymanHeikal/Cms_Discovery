using MediatR;
using ContentManagementSystem.Core.Queries;
using ContentManagementSystem.Core.DTOs;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Interfaces;

namespace ContentManagementSystem.Infrastructure.Handlers;

public class SearchProgramsQueryHandler : IRequestHandler<SearchProgramsQuery, SearchResult<Program>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProgramsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SearchResult<Program>> Handle(SearchProgramsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Programs.SearchAsync(request.Criteria, cancellationToken);
    }
}