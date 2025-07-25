using MediatR;
using Microsoft.EntityFrameworkCore;
using ContentManagementSystem.Core.Commands;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Enums;
using ContentManagementSystem.Core.Interfaces;

namespace ContentManagementSystem.Infrastructure.Handlers;

public class UpdateProgramCommandHandler : IRequestHandler<UpdateProgramCommand, Program>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProgramCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Program> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var program = await _unitOfWork.Programs.GetByIdAsync(request.Id, cancellationToken);
            if (program == null)
            {
                throw new InvalidOperationException("Program not found");
            }

            // Update program properties
            program.Title = request.Title;
            program.Description = request.Description;
            program.ThumbnailUrl = request.ThumbnailUrl;
            program.VideoUrl = request.VideoUrl;
            program.Duration = request.Duration;
            program.PublishedDate = request.PublishedDate;
            program.Type = (ProgramType)request.Type;
            program.Language = (Language)request.Language;
            program.Status = (ProgramStatus)request.Status;
            program.UpdatedBy = request.UpdatedBy;

            // Clear existing categories and tags
            program.ProgramCategories.Clear();
            program.ProgramTags.Clear();

            // Add new categories
            foreach (var categoryId in request.CategoryIds)
            {
                program.ProgramCategories.Add(new ProgramCategory
                {
                    ProgramId = program.Id,
                    CategoryId = categoryId
                });
            }

            // Add new tags
            foreach (var tagId in request.TagIds)
            {
                program.ProgramTags.Add(new ProgramTag
                {
                    ProgramId = program.Id,
                    TagId = tagId
                });
            }

            await _unitOfWork.Programs.UpdateAsync(program, cancellationToken);
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