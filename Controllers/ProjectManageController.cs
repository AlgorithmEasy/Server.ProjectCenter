using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AlgorithmEasy.Server.ProjectCenter.Services;
using AlgorithmEasy.Server.ProjectCenter.Statuses;
using AlgorithmEasy.Shared.Requests;
using AlgorithmEasy.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgorithmEasy.Server.ProjectCenter.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ProjectManageController : ControllerBase
    {
        private readonly ProjectManageService _projectManager;

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public ProjectManageController(ProjectManageService projectManager) => _projectManager = projectManager;

        [HttpGet]
        public ActionResult<GetPersonalProjectsResponse> GetPersonalProjects()
        {
            if (UserId == null)
                return Unauthorized();
            var response = new GetPersonalProjectsResponse
            {
                Projects = _projectManager.GetPersonalProjects(UserId)
            };
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<string> CreateProject([Required][FromBody] CreateProjectRequest request)
        {
            if (UserId == null)
                return Unauthorized();
            if (_projectManager.CreateProject(UserId, request.ProjectName))
                return Ok($"{request.ProjectName}项目创建成功。");
            return BadRequest($"{request.ProjectName}项目创建失败，请稍后重试。");
        }

        [HttpPut]
        public ActionResult<string> SaveProject([Required] string projectName, [FromBody] string workspace)
        {
            if (UserId == null)
                return Unauthorized();
            if (_projectManager.SaveProject(UserId, projectName, workspace))
                return Ok($"{projectName}项目保存成功。");
            return BadRequest($"找不到{projectName}项目，请稍后重试。");
        }

        [HttpPut]
        public ActionResult<string> RenameProject([Required][FromBody] RenameProjectRequest request)
        {
            if (UserId == null)
                return Unauthorized();
            switch (_projectManager.RenameProject(UserId, request.OldProjectName, request.NewProjectName))
            {
                case UpdateProjectNameStatus.NoOldProject:
                    return BadRequest($"找不到{request.OldProjectName}项目，请刷新后重试。");
                case UpdateProjectNameStatus.ConflictNewName:
                    return BadRequest($"{request.NewProjectName}与已有项目冲突，请重新命名后提交。");
                default:
                    return Ok($"{request.OldProjectName}项目成功更名为{request.NewProjectName}项目。");
            }
        }

        [HttpDelete]
        public ActionResult<string> DeleteProject([Required] string projectName)
        {
            if (UserId == null)
                return Unauthorized();
            if (_projectManager.DeleteProject(UserId, projectName))
                return Ok($"{projectName}项目删除成功。");
            return BadRequest($"{projectName}项目删除失败，请稍后重试。");
        }

    }
}