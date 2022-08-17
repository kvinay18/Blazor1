using AenonSrp.Application.Features.ProjectDetails.Commands.ArchiveProject;
using AenonSrp.Application.Features.ProjectDetails.Commands.Delete;
using AenonSrp.Application.Features.ProjectDetails.Commands.UpdateTeamLead;
using AenonSrp.Application.Features.ProjectDetails.Queries.ExportToCSV;
using AenonSrp.Application.Features.ProjectDetails.Queries.GetById;
using AenonSrp.Application.Features.ProjectDetails.Queries.GetCalc;
using AenonSrp.Application.Features.ProjectDetails.Queries.GetUserProjects;
using AenonSrp.Application.Requests.Project;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace AenonSrp.Server.Controllers.v1.Project
{
    public class ProjectDetailController : BaseApiController<ProjectDetailController>
    {
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ProjectDetailPagingRequest request)
        {
            var projects = await _mediator.Send(request);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _mediator.Send(new GetProjectDetailByIdQuery() { Id = id });
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProjectDetailRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("update-team-lead")]
        public async Task<IActionResult> UpdateProjectTeamLead(UpdateProjectTeamLeadRequest request)
        {
            return Ok(await _mediator.Send(new UpdateTeamLeadCommand() { ProjectId = request.Id, TeamLeadId = request.ProjectLeadId }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteProjectDetailCommand { Id = id }));
        }

        [HttpGet("GetCalculatedCount")]
        public async Task<IActionResult> GetCalculatedCount()
        {
            return Ok(await _mediator.Send(new GetProjectDetailCalcCountQuery() { }));
        }

        [HttpGet("getalluserproject")]
        public async Task<IActionResult> GetAllUserProject()
        {
            var users = await _mediator.Send(new GetAllUserProjectQuery());
            return Ok(users);
        }

        [HttpGet("archiveproject")]
        public async Task<IActionResult> ArchiveProject(int projectId, bool isArchived)
        {
            return Ok(await _mediator.Send(new ArchiveProjectCommand() { ProjectId = projectId, IsArchived = isArchived }));
        }
        
        [HttpGet("exporttoexcel")]
        public async Task<IActionResult> ExportProjects()
        {
            var response = await _mediator.Send(new ExportProjectDetailQuery());
            var stream = new MemoryStream();
            using (var writeFile = new StreamWriter(stream, leaveOpen: true))
            {
                var csv = new CsvWriter(writeFile, new CsvConfiguration(new System.Globalization.CultureInfo("en-US")));
                csv.WriteRecords(response.Data);
            }
            stream.Position = 0; //reset stream
            return File(stream, "application/octet-stream");
            return Ok(await _mediator.Send(new ExportProjectDetailQuery()));
        }

    }
}
