using Microsoft.EntityFrameworkCore;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Interfaces;
using ContentManagementSystem.Core.DTOs;
using ContentManagementSystem.Core.Enums;
using ContentManagementSystem.Infrastructure.Data;

namespace ContentManagementSystem.Infrastructure.Repositories;

public class ProgramRepository : Repository<Program>, IProgramRepository
{
    public ProgramRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Program?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<SearchResult<Program>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm));
        }

        if (criteria.Type.HasValue)
        {
            query = query.Where(p => (int)p.Type == criteria.Type.Value);
        }

        if (criteria.Language.HasValue)
        {
            query = query.Where(p => (int)p.Language == criteria.Language.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == criteria.Status.Value);
        }
        else
        {
            // By default, only show published content for discovery
            query = query.Where(p => p.Status == ProgramStatus.Published);
        }

        if (criteria.CategoryIds.Any())
        {
            query = query.Where(p => p.ProgramCategories.Any(pc => criteria.CategoryIds.Contains(pc.CategoryId)));
        }

        if (criteria.TagIds.Any())
        {
            query = query.Where(p => p.ProgramTags.Any(pt => criteria.TagIds.Contains(pt.TagId)));
        }

        if (criteria.FromDate.HasValue)
        {
            query = query.Where(p => p.PublishedDate >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            query = query.Where(p => p.PublishedDate <= criteria.ToDate.Value);
        }

        // Apply sorting
        query = criteria.SortBy.ToLower() switch
        {
            "title" => criteria.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "publisheddate" => criteria.SortDescending ? query.OrderByDescending(p => p.PublishedDate) : query.OrderBy(p => p.PublishedDate),
            "viewcount" => criteria.SortDescending ? query.OrderByDescending(p => p.ViewCount) : query.OrderBy(p => p.ViewCount),
            "rating" => criteria.SortDescending ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),
            "duration" => criteria.SortDescending ? query.OrderByDescending(p => p.Duration) : query.OrderBy(p => p.Duration),
            _ => criteria.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new SearchResult<Program>
        {
            Items = items,
            TotalCount = totalCount,
            Page = criteria.Page,
            PageSize = criteria.PageSize
        };
    }

    public async Task<IEnumerable<Program>> GetByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => (int)p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Program>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.ProgramCategories.Any(pc => pc.CategoryId == categoryId) && p.Status == ProgramStatus.Published)
            .OrderByDescending(p => p.PublishedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Program>> GetByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.ProgramTags.Any(pt => pt.TagId == tagId) && p.Status == ProgramStatus.Published)
            .OrderByDescending(p => p.PublishedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Program>> GetMostViewedAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == ProgramStatus.Published)
            .OrderByDescending(p => p.ViewCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Program>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProgramCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProgramTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == ProgramStatus.Published)
            .OrderByDescending(p => p.PublishedDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task IncrementViewCountAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        var program = await _dbSet.FindAsync(new object[] { programId }, cancellationToken);
        if (program != null)
        {
            program.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}