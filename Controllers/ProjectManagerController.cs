using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AlgorithmEasy.Server.ProjectCenter.Services;
using AlgorithmEasy.Server.ProjectCenter.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgorithmEasy.Server.ProjectCenter.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ProjectManagerController : ControllerBase
    {
        private readonly ProjectManagerService _projectManager;

        public ProjectManagerController(ProjectManagerService projectManager) => _projectManager = projectManager;

        [HttpPost]
        public ActionResult CreateProject([Required] string projectName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.ToString();
            if (userId == null)
                return new UnauthorizedResult();
            if (_projectManager.CreateProject(userId, projectName))
                return Ok($"{projectName}项目创建成功。");
            return BadRequest($"{projectName}项目创建失败，请稍后重试。");
        }

        [HttpPut]
        public ActionResult UpdateWorkspace([Required] string projectName, [FromForm] string workspace)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.ToString();
            if (userId == null)
                return new UnauthorizedResult();
            if (_projectManager.UpdateWorkspace(userId, projectName, workspace))
                return Ok($"{projectName}项目保存成功。");
            return BadRequest($"找不到{projectName}项目，请稍后重试。");
        }

        [HttpPut]
        public ActionResult UpdateProjectName([Required] string oldName, [Required] string newName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.ToString();
            if (userId == null)
                return new UnauthorizedResult();
            switch (_projectManager.UpdateProjectName(userId, oldName, newName))
            {
                case UpdateProjectNameStatus.NoOldProject:
                    return BadRequest($"找不到{oldName}项目，请刷新后重试。");
                case UpdateProjectNameStatus.ConflictNewName:
                    return BadRequest($"{newName}与已有项目冲突，请重新命名后提交。");
                default:
                    return Ok($"{oldName}项目成功更名为{newName}项目。");
            }
        }

        [HttpDelete]
        public ActionResult DeleteProject([Required] string projectName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.ToString();
            if (userId == null)
                return new UnauthorizedResult();
            if (_projectManager.DeleteProject(userId, projectName))
                return Ok($"{projectName}项目删除成功。");
            return BadRequest($"{projectName}项目删除失败，请稍后重试。");
        }

    }
}