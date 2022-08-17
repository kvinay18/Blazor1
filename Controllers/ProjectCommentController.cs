using AenonSrp.Application.Features.ProjectComments.Commands.Delete;
using AenonSrp.Application.Features.ProjectComments.Queries.GetAllByProjectId;
using AenonSrp.Application.Features.ProjectComments.Queries.GetById;
using AenonSrp.Application.Requests.Project;
using Microsoft.AspNetCore.Mvc;

namespace AenonSrp.Server.Controllers.v1.Project
{
    public class ProjectCommentController : BaseApiController<ProjectDetailController>
    {
        [HttpGet("commentbyprojectid")]
        public async Task<IActionResult> GetAllByProjectId(int projectId, int pageNumber, int pageSize)
        {
            var Checklists = await _mediator.Send(new GetAllProjectByProjectIdQuery() { ProjectId = projectId, PageNumber = pageNumber, PageSize = pageSize });
            return Ok(Checklists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Checklist = await _mediator.Send(new GetProjectCommentByIdQuery() { Id = id });
            return Ok(Checklist);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProjectCommentRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteProjectCommentCommand { Id = id }));
        }
    }
}
