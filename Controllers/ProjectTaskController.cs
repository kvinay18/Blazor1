using AenonSrp.Application.Features.ProjectTasks.Commands.Delete;
using AenonSrp.Application.Features.ProjectTasks.Queries.GetAll;
using AenonSrp.Application.Features.ProjectTasks.Queries.GetById;
using AenonSrp.Application.Features.ProjectTasks.Queries.GetCountCalculation;
using AenonSrp.Application.Requests.Project;
using Microsoft.AspNetCore.Mvc;

namespace AenonSrp.Server.Controllers.v1.Project
{
    public class ProjectTaskController : BaseApiController<ProjectDetailController>
    {
        [HttpPost("taskbyprojectid")]
        public async Task<IActionResult> GetAllByProjectId(ProjectTaskPagingRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _mediator.Send(new GetProjectTaskByIdQuery() { Id = id });
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProjectTaskRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteProjectTaskCommand { Id = id }));
        }

        [HttpGet("gettaskcalculatedcount")]
        public async Task<IActionResult> GetTaskCalculatedCount()
        {
            return Ok(await _mediator.Send(new GetProjectTaskCountCalculationQuery() { }));
        }
    }
}
