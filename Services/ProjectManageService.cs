using System.Collections.Generic;
using System.Linq;
using AlgorithmEasy.Server.ProjectCenter.Statuses;
using AlgorithmEasy.Shared.Models;

namespace AlgorithmEasy.Server.ProjectCenter.Services
{
    public class ProjectManageService
    {
        private readonly AlgorithmEasyDbContext _dbContext;

        public ProjectManageService(AlgorithmEasyDbContext dbContext) => _dbContext = dbContext;

        public IEnumerable<Project> GetPersonalProjects(string userId)
        {
            return _dbContext.Projects
                .Where(project => project.UserId == userId)
                .OrderByDescending(project => project.UpdateTime)
                .ToList();
        }

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

        public bool SaveProject(string userId, string projectName, string workspace)
        {
            var project = _dbContext.Projects.SingleOrDefault(p =>
                p.UserId == userId && p.ProjectName == projectName);
            if (project == null)
                return false;

            project.Workspace = workspace;
            _dbContext.SaveChanges();
            return true;
        }

        public UpdateProjectNameStatus RenameProject(string userId, string oldProjectName, string newProjectName)
        {
            var project = _dbContext.Projects.SingleOrDefault(p =>
                p.UserId == userId && p.ProjectName == oldProjectName);
            if (project == null)
                return UpdateProjectNameStatus.NoOldProject;

            if (_dbContext.Projects.Any(p => p.UserId == userId && p.ProjectName == newProjectName))
                return UpdateProjectNameStatus.ConflictNewName;

            var newProject = new Project
            {
                UserId = project.UserId,
                ProjectName = newProjectName,
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