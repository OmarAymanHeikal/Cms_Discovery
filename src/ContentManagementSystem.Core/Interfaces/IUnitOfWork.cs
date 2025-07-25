using ContentManagementSystem.Core.Entities;

namespace ContentManagementSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProgramRepository Programs { get; }
    IRepository<Category> Categories { get; }
    IRepository<Tag> Tags { get; }
    IRepository<Comment> Comments { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}