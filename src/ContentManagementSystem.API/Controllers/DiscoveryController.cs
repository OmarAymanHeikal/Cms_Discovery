using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ContentManagementSystem.API.DTOs;
using ContentManagementSystem.Core.Queries;
using ContentManagementSystem.Core.DTOs;
using ContentManagementSystem.Core.Enums;

namespace ContentManagementSystem.API.Controllers;

/// <summary>
/// Discovery API - For public content discovery and search
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DiscoveryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public DiscoveryController(IMediator mediator, IMapper mapper, IMemoryCache cache)
    {
        _mediator = mediator;
        _mapper = mapper;
        _cache = cache;
    }

    /// <summary>
    /// Search published programs for public discovery
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="type">Program type filter</param>
    /// <param name="language">Language filter</param>
    /// <param name="categoryIds">Category IDs filter</param>
    /// <param name="tagIds">Tag IDs filter</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDesc">Sort descending</param>
    /// <returns>Paginated search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchResult<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResult<ProgramDto>>> SearchPrograms(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? type = null,
        [FromQuery] int? language = null,
        [FromQuery] string? categoryIds = null,
        [FromQuery] string? tagIds = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDesc = true)
    {
        var criteria = new SearchCriteria
        {
            SearchTerm = searchTerm,
            Type = type,
            Language = language,
            Status = (int)ProgramStatus.Published, // Only published content for discovery
            CategoryIds = ParseGuids(categoryIds),
            TagIds = ParseGuids(tagIds),
            Page = page,
            PageSize = Math.Min(pageSize, 50), // Limit page size for performance
            SortBy = sortBy,
            SortDescending = sortDesc
        };

        var cacheKey = $"search_{criteria.GetHashCode()}";
        
        if (!_cache.TryGetValue(cacheKey, out SearchResult<ProgramDto>? cachedResult))
        {
            var query = new SearchProgramsQuery { Criteria = criteria };
            var result = await _mediator.Send(query);
            
            cachedResult = new SearchResult<ProgramDto>
            {
                Items = _mapper.Map<IEnumerable<ProgramDto>>(result.Items),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            _cache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(5));
        }

        return Ok(cachedResult);
    }

    /// <summary>
    /// Get program details by ID (public view)
    /// </summary>
    /// <param name="id">Program ID</param>
    /// <returns>Program details</returns>
    [HttpGet("programs/{id}")]
    [ProducesResponseType(typeof(ProgramDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgramDto>> GetProgram(Guid id)
    {
        var query = new GetProgramByIdQuery { Id = id };
        var program = await _mediator.Send(query);
        
        if (program == null || program.Status != ProgramStatus.Published)
        {
            return NotFound();
        }
        
        var programDto = _mapper.Map<ProgramDto>(program);
        return Ok(programDto);
    }

    /// <summary>
    /// Get programs by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Programs in category</returns>
    [HttpGet("categories/{categoryId}/programs")]
    [ProducesResponseType(typeof(SearchResult<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResult<ProgramDto>>> GetProgramsByCategory(
        Guid categoryId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var criteria = new SearchCriteria
        {
            CategoryIds = new List<Guid> { categoryId },
            Status = (int)ProgramStatus.Published,
            Page = page,
            PageSize = Math.Min(pageSize, 50),
            SortBy = "PublishedDate",
            SortDescending = true
        };

        var query = new SearchProgramsQuery { Criteria = criteria };
        var result = await _mediator.Send(query);
        
        var dtoResult = new SearchResult<ProgramDto>
        {
            Items = _mapper.Map<IEnumerable<ProgramDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(dtoResult);
    }

    /// <summary>
    /// Get programs by tag
    /// </summary>
    /// <param name="tagId">Tag ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Programs with tag</returns>
    [HttpGet("tags/{tagId}/programs")]
    [ProducesResponseType(typeof(SearchResult<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResult<ProgramDto>>> GetProgramsByTag(
        Guid tagId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var criteria = new SearchCriteria
        {
            TagIds = new List<Guid> { tagId },
            Status = (int)ProgramStatus.Published,
            Page = page,
            PageSize = Math.Min(pageSize, 50),
            SortBy = "PublishedDate",
            SortDescending = true
        };

        var query = new SearchProgramsQuery { Criteria = criteria };
        var result = await _mediator.Send(query);
        
        var dtoResult = new SearchResult<ProgramDto>
        {
            Items = _mapper.Map<IEnumerable<ProgramDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(dtoResult);
    }

    /// <summary>
    /// Get most viewed programs
    /// </summary>
    /// <param name="count">Number of programs to return</param>
    /// <returns>Most viewed programs</returns>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(IEnumerable<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProgramDto>>> GetTrendingPrograms([FromQuery] int count = 10)
    {
        const string cacheKey = "trending_programs";
        
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProgramDto>? cachedResult))
        {
            var criteria = new SearchCriteria
            {
                Status = (int)ProgramStatus.Published,
                PageSize = Math.Min(count, 50),
                SortBy = "ViewCount",
                SortDescending = true
            };

            var query = new SearchProgramsQuery { Criteria = criteria };
            var result = await _mediator.Send(query);
            
            cachedResult = _mapper.Map<IEnumerable<ProgramDto>>(result.Items);
            _cache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(15));
        }

        return Ok(cachedResult);
    }

    /// <summary>
    /// Get recently published programs
    /// </summary>
    /// <param name="count">Number of programs to return</param>
    /// <returns>Recently published programs</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProgramDto>>> GetRecentPrograms([FromQuery] int count = 10)
    {
        const string cacheKey = "recent_programs";
        
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProgramDto>? cachedResult))
        {
            var criteria = new SearchCriteria
            {
                Status = (int)ProgramStatus.Published,
                PageSize = Math.Min(count, 50),
                SortBy = "PublishedDate",
                SortDescending = true
            };

            var query = new SearchProgramsQuery { Criteria = criteria };
            var result = await _mediator.Send(query);
            
            cachedResult = _mapper.Map<IEnumerable<ProgramDto>>(result.Items);
            _cache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(10));
        }

        return Ok(cachedResult);
    }

    private static List<Guid> ParseGuids(string? guidString)
    {
        if (string.IsNullOrWhiteSpace(guidString))
            return new List<Guid>();

        return guidString.Split(',')
            .Where(s => Guid.TryParse(s.Trim(), out _))
            .Select(s => Guid.Parse(s.Trim()))
            .ToList();
    }
}