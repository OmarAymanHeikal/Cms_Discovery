using MediatR;
using ContentManagementSystem.Core.DTOs;
using ContentManagementSystem.Core.Entities;

namespace ContentManagementSystem.Core.Queries;

public class SearchProgramsQuery : IRequest<SearchResult<Program>>
{
    public SearchCriteria Criteria { get; set; } = new();
}