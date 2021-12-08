using System.Linq;
using AlgorithmEasy.Server.ProjectCenter.Status;
using AlgorithmEasy.Shared.Models;

namespace AlgorithmEasy.Server.ProjectCenter.Services
{
    public class ProjectManagerService
    {
        private readonly AlgorithmEasyDbContext _dbContext;

        public ProjectManagerService(AlgorithmEasyDbContext dbContext) => _dbContext = dbContext;
        public bool CreateProject(string userId, string projectName)
        {
            if (_dbContext.Projects.Any(p => p.UserId == userId && p.ProjectName == projectName))
                return false;

            var project = new Project
            {
                UserId = userId,
                ProjectName = projectName
            };
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();
            return true;
        }

        public bool UpdateWorkspace(string userId, string projectName, string workspace)
        {
            var project = _dbContext.Projects.SingleOrDefault(p =>
                p.UserId == userId && p.ProjectName == projectName);
            if (project == null)
                return false;

            project.Workspace = workspace;
            _dbContext.Projects.Update(project);
            _dbContext.SaveChanges();
            return true;
        }

        public UpdateProjectNameStatus UpdateProjectName(string userId, string oldName, string newName)
        {
            var project = _dbContext.Projects.SingleOrDefault(p =>
                p.UserId == userId && p.ProjectName == oldName);
            if (project == null)
                return UpdateProjectNameStatus.NoOldProject;

            if (_dbContext.Projects.Any(p => p.UserId == userId && p.ProjectName == newName))
                return UpdateProjectNameStatus.ConflictNewName;

            var newProject = new Project
            {
                UserId = project.UserId,
                ProjectName = newName,
                Workspace = project.Workspace,
            };
            _dbContext.Projects.Remove(project);
            _dbContext.Projects.Add(newProject);
            _dbContext.SaveChanges();
            return UpdateProjectNameStatus.Success;
        }

        public bool DeleteProject(string userId, string projectName)
        {
            var project = _dbContext.Projects.SingleOrDefault(p =>
                p.UserId == userId && p.ProjectName == projectName);
            if (project == null)
                return false;

            _dbContext.Projects.Remove(project);
            _dbContext.SaveChanges();
            return true;
        }
    }
}