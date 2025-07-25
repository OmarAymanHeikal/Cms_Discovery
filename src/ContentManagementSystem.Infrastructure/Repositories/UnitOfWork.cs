using Microsoft.EntityFrameworkCore.Storage;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Interfaces;
using ContentManagementSystem.Infrastructure.Data;

namespace ContentManagementSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IProgramRepository? _programs;
    private IRepository<Category>? _categories;
    private IRepository<Tag>? _tags;
    private IRepository<Comment>? _comments;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProgramRepository Programs =>
        _programs ??= new ProgramRepository(_context);

    public IRepository<Category> Categories =>
        _categories ??= new Repository<Category>(_context);

    public IRepository<Tag> Tags =>
        _tags ??= new Repository<Tag>(_context);

    public IRepository<Comment> Comments =>
        _comments ??= new Repository<Comment>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}