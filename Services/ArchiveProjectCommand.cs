using AenonSrp.Application.Interfaces.Repositories;
using AenonSrp.Domain.Entities.Project;
using AenonSrp.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace AenonSrp.Application.Features.ProjectDetails.Commands.ArchiveProject
{
    public partial class ArchiveProjectCommand : IRequest<Result<int>>
    {
        public int ProjectId { get; set; }
        public bool IsArchived { get; set; }
    }

    internal class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand, Result<int>>
    {
        private readonly IStringLocalizer<ArchiveProjectCommand> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public ArchiveProjectCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ArchiveProjectCommand> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(ArchiveProjectCommand command, CancellationToken cancellationToken)
        {
            var projectDetails = await _unitOfWork.Repository<ProjectDetail>()
                                                    .Entities
                                                    .Include(x => x.ProjectMembers)
                                                    .Include(x => x.ProjectTasks)
                                                    .FirstOrDefaultAsync(x => x.Id == command.ProjectId && !x.IsDeleted);
            if (projectDetails != null)
            {
                projectDetails.IsArchived = command.IsArchived;
                if (command.IsArchived)
                {
                    projectDetails.ProjectLeadId = null;
                    projectDetails.ProjectMembers = projectDetails.ProjectMembers.Select(x => { x.IsDeleted = true; return x; }).ToList();
                    projectDetails.ProjectTasks = projectDetails.ProjectTasks.Select(x => { x.AssigneeId = null; return x; }).ToList();
                }
                //await _unitOfWork.Repository<Module>().DeleteAsync(Module);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(projectDetails.Id, _localizer["Project Archived"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Project Not Found!"]);
            }
        }
    }
}
