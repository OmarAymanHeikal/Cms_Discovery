using MediatR;
using ContentManagementSystem.Core.Commands;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Enums;
using ContentManagementSystem.Core.Interfaces;

namespace ContentManagementSystem.Infrastructure.Handlers;

public class CreateProgramCommandHandler : IRequestHandler<CreateProgramCommand, Program>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProgramCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Program> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var program = new Program
            {
                Title = request.Title,
                Description = request.Description,
                ThumbnailUrl = request.ThumbnailUrl,
                VideoUrl = request.VideoUrl,
                Duration = request.Duration,
                PublishedDate = request.PublishedDate,
                Type = (ProgramType)request.Type,
                Language = (Language)request.Language,
                Status = (ProgramStatus)request.Status,
                CreatedBy = request.CreatedBy,
                UpdatedBy = request.CreatedBy
            };

            await _unitOfWork.Programs.AddAsync(program, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add categories
            foreach (var categoryId in request.CategoryIds)
            {
                var programCategory = new ProgramCategory
                {
                    ProgramId = program.Id,
                    CategoryId = categoryId
                };
                await _unitOfWork.Programs.AddAsync(program, cancellationToken);
            }

            // Add tags
            foreach (var tagId in request.TagIds)
            {
                var programTag = new ProgramTag
                {
                    ProgramId = program.Id,
                    TagId = tagId
                };
                await _unitOfWork.Programs.AddAsync(program, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return await _unitOfWork.Programs.GetByIdAsync(program.Id, cancellationToken) ?? program;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}