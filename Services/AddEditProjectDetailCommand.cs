using AenonSrp.Application.Interfaces.Repositories;
using AenonSrp.Application.Interfaces.Services;
using AenonSrp.Application.Interfaces.Services.GlobalSetting;
using AenonSrp.Application.Requests.Project;
using AenonSrp.Domain.Entities.Project;
using AenonSrp.Shared.Constants.GlobalSetting;
using AenonSrp.Shared.Constants.ProjectConstants;
using AenonSrp.Shared.Wrapper;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using project = AenonSrp.Domain.Entities.Project;

namespace AenonSrp.Application.Features.ProjectDetails.Commands.AddEdit
{
    public class AddEditProjectDetailCommandHandler : IRequestHandler<ProjectDetailRequest, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditProjectDetailCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGlobalSettingService _globalSettingService;

        public AddEditProjectDetailCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IMapper mapper,
            IStringLocalizer<AddEditProjectDetailCommandHandler> localizer,
            ICurrentUserService currentUserService,
            IGlobalSettingService globalSettingService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _currentUserService = currentUserService;
            _globalSettingService = globalSettingService;
        }

        public async Task<Result<int>> Handle(ProjectDetailRequest command, CancellationToken cancellationToken)
        {
            var mappedProjectDetail = _mapper.Map<ProjectDetail>(command);
            if (mappedProjectDetail.Id == 0)
            {
                var response = await _unitOfWork.Repository<ProjectDetail>().AddAsync(mappedProjectDetail);
                var id = await _unitOfWork.Commit(cancellationToken);

                //Get Prefix from global setting
                var globalSetting = _globalSettingService.GetGlobalSettingValueByName(GlobalSettingConstatnt.CalcProjectId);

                //set Project Id
                response.CalcProjectId = $"{globalSetting?.Value}{response.Id.ToString(ProjectConstant.SufixFormat)}";
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(response.Id, _localizer["Project Saved"]);
            }
            else
            {
                var projectDetail = await _unitOfWork.Repository<ProjectDetail>().Entities
                                .Include(x => x.ProjectTags)
                                .Where(x => x.Id == mappedProjectDetail.Id && !x.IsDeleted)
                                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if (projectDetail == null)
                {
                    return await Result<int>.FailAsync(_localizer["Project Not Found!"]);
                }
                else
                {

                    command.ProjectMembers = null;
                    command.ProjectTags = null;
                    _mapper.Map<ProjectDetailRequest, ProjectDetail>(command, projectDetail);

                    await _unitOfWork.Repository<ProjectDetail>().UpdateAsync(projectDetail);


                    // Delete project tags 
                    if (projectDetail.ProjectTags?.Count > 0)
                    {
                        await _unitOfWork.Repository<ProjectTag>().DeleteRangeAsync(projectDetail.ProjectTags);
                    }

                    //add project tags
                    if (mappedProjectDetail.ProjectTags?.Count > 0)
                        await _unitOfWork.Repository<ProjectTag>().AddRangeAsync(mappedProjectDetail.ProjectTags);

                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<int>.SuccessAsync(projectDetail.Id, _localizer["Project Updated"]);

                }
            }
        }
    }
}
