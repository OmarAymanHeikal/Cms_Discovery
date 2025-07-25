using MediatR;
using ContentManagementSystem.Core.Commands;
using ContentManagementSystem.Core.Interfaces;

namespace ContentManagementSystem.Infrastructure.Handlers;

public class DeleteProgramCommandHandler : IRequestHandler<DeleteProgramCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProgramCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var program = await _unitOfWork.Programs.GetByIdAsync(request.Id, cancellationToken);
            if (program == null)
            {
                return false;
            }

            program.IsDeleted = true;
            program.UpdatedBy = request.DeletedBy;
            program.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Programs.UpdateAsync(program, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}