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
    public class ProjectManageController : ControllerBase
    {
        private readonly ProjectManageService _projectManage;

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public ProjectManageController(ProjectManageService projectManage) => _projectManage = projectManage;

        [HttpGet]
        public ActionResult GetPersonalProjects()
        {
            if (UserId == null)
                return Unauthorized();
            return Ok(_projectManage.GetPersonalProjects(UserId));
        }

        [HttpPost]
        public ActionResult CreateProject([Required] string projectName)
        {
            if (UserId == null)
                return Unauthorized();
            if (_projectManage.CreateProject(UserId, projectName))
                return Ok($"{projectName}项目创建成功。");
            return BadRequest($"{projectName}项目创建失败，请稍后重试。");
        }

        [HttpPut]
        public ActionResult UpdateWorkspace([Required] string projectName, [FromForm] string workspace)
        {
            if (UserId == null)
                return Unauthorized();
            if (_projectManage.UpdateWorkspace(UserId, projectName, workspace))
                return Ok($"{projectName}项目保存成功。");
            return BadRequest($"找不到{projectName}项目，请稍后重试。");
        }

        [HttpPut]
        public ActionResult UpdateProjectName([Required] string oldName, [Required] string newName)
        {
            if (UserId == null)
                return Unauthorized();
            switch (_projectManage.UpdateProjectName(UserId, oldName, newName))
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
            if (UserId == null)
                return Unauthorized();
            if (_projectManage.DeleteProject(UserId, projectName))
                return Ok($"{projectName}项目删除成功。");
            return BadRequest($"{projectName}项目删除失败，请稍后重试。");
        }

    }
}