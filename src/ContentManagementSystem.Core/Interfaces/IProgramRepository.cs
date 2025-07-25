using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.DTOs;

namespace ContentManagementSystem.Core.Interfaces;

public interface IProgramRepository : IRepository<Program>
{
    Task<SearchResult<Program>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<IEnumerable<Program>> GetByStatusAsync(int status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Program>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Program>> GetByTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Program>> GetMostViewedAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Program>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task IncrementViewCountAsync(Guid programId, CancellationToken cancellationToken = default);
}