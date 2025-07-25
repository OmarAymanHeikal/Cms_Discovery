using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using ContentManagementSystem.API.DTOs;
using ContentManagementSystem.Core.Commands;
using ContentManagementSystem.Core.Queries;
using ContentManagementSystem.Core.DTOs;

namespace ContentManagementSystem.API.Controllers;

/// <summary>
/// Content Management System API - For internal use by content editors and managers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CMSController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CMSController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new program
    /// </summary>
    /// <param name="request">Program creation request</param>
    /// <returns>Created program</returns>
    [HttpPost("programs")]
    [ProducesResponseType(typeof(ProgramDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProgramDto>> CreateProgram([FromBody] CreateProgramRequest request)
    {
        var command = _mapper.Map<CreateProgramCommand>(request);
        command.CreatedBy = User.Identity?.Name ?? "System";
        
        var program = await _mediator.Send(command);
        var programDto = _mapper.Map<ProgramDto>(program);
        
        return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, programDto);
    }

    /// <summary>
    /// Update an existing program
    /// </summary>
    /// <param name="id">Program ID</param>
    /// <param name="request">Program update request</param>
    /// <returns>Updated program</returns>
    [HttpPut("programs/{id}")]
    [ProducesResponseType(typeof(ProgramDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProgramDto>> UpdateProgram(Guid id, [FromBody] UpdateProgramRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("ID mismatch");
        }

        var command = _mapper.Map<UpdateProgramCommand>(request);
        command.UpdatedBy = User.Identity?.Name ?? "System";
        
        var program = await _mediator.Send(command);
        var programDto = _mapper.Map<ProgramDto>(program);
        
        return Ok(programDto);
    }

    /// <summary>
    /// Delete a program (soft delete)
    /// </summary>
    /// <param name="id">Program ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("programs/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProgram(Guid id)
    {
        var command = new DeleteProgramCommand 
        { 
            Id = id, 
            DeletedBy = User.Identity?.Name ?? "System" 
        };
        
        var result = await _mediator.Send(command);
        
        if (!result)
        {
            return NotFound();
        }
        
        return NoContent();
    }

    /// <summary>
    /// Get program by ID (includes all details for CMS)
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
        
        if (program == null)
        {
            return NotFound();
        }
        
        var programDto = _mapper.Map<ProgramDto>(program);
        return Ok(programDto);
    }

    /// <summary>
    /// Search programs with full CMS capabilities (includes draft, under review, etc.)
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Paginated search results</returns>
    [HttpPost("programs/search")]
    [ProducesResponseType(typeof(SearchResult<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResult<ProgramDto>>> SearchPrograms([FromBody] SearchProgramsRequest request)
    {
        var criteria = _mapper.Map<SearchCriteria>(request);
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
    /// Get programs by status (for workflow management)
    /// </summary>
    /// <param name="status">Program status (1=Draft, 2=UnderReview, 3=Published, 4=Archived, 5=Rejected)</param>
    /// <returns>Programs with specified status</returns>
    [HttpGet("programs/status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<ProgramDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProgramDto>>> GetProgramsByStatus(int status)
    {
        var criteria = new SearchCriteria { Status = status, PageSize = 100 };
        var query = new SearchProgramsQuery { Criteria = criteria };
        
        var result = await _mediator.Send(query);
        var dtoResult = _mapper.Map<IEnumerable<ProgramDto>>(result.Items);
        
        return Ok(dtoResult);
    }
}