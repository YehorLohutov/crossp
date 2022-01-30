using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Projects.Models;
using System;
using System.Linq;

namespace Projects.Repository
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ProjectsDatabaseContext databaseContext = default;

        public ProjectsRepository(ProjectsDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        #region IProjectsRepository
        public async Task<IEnumerable<Project>> GetProjectsAsync() => await databaseContext.Projects.ToArrayAsync();
        
        public async Task<Project> GetProjectAsync(int projectId)
        {
            Project project = await databaseContext.Projects.FirstOrDefaultAsync(t => t.Id == projectId);

            if (project == default)
                throw new NullReferenceException($"{nameof(Project)} with {nameof(projectId)} = {projectId} not exist.");

            return project;
        }

        public async Task<Project> GetProjectAsync(string projectExternalId)
        {
            Project project = await databaseContext.Projects.FirstOrDefaultAsync(t => t.ExternalId == projectExternalId);

            if (project == default)
                throw new NullReferenceException($"{nameof(Project)} with {nameof(projectExternalId)} = {projectExternalId} not exist.");

            return project;
        }

        public async Task<IEnumerable<Project>> GetUserProjectsAsync(string userExternalId)
        {
            if (string.IsNullOrWhiteSpace(userExternalId))
                throw new ArgumentException($"Parameter {nameof(userExternalId)} passed into {nameof(GetUserProjectsAsync)} is null or empty.");
            
            return await databaseContext.Projects.Where(project => project.UserExternalId.Equals(userExternalId)).ToArrayAsync();
        }

        public async Task<bool> ProjectExists(string projectExternalId) =>
            await databaseContext.Projects.AnyAsync(t => t.ExternalId.Equals(projectExternalId));
        

        public void DeleteProject(Project project)
        {
            if (project == default)
                throw new NullReferenceException($"Parameter {nameof(project)} passed into {nameof(DeleteProject)} is null.");

            databaseContext.Projects.Remove(project);
        }

        public void UpdateProject(Project project)
        {
            if (project == default)
                throw new NullReferenceException($"Parameter {nameof(project)} passed into {nameof(UpdateProject)} is null.");

            databaseContext.Projects.Update(project);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            Project project = await GetProjectAsync(projectId);
            DeleteProject(project);
        }

        public async Task DeleteProjectAsync(string projectExternalId)
        {
            Project project = await GetProjectAsync(projectExternalId);
            DeleteProject(project);
        }

        public async Task InsertProjectAsync(Project project)
        {
            if (project == default)
                throw new NullReferenceException($"Parameter {nameof(project)} passed into {nameof(InsertProjectAsync)} is null.");
            await databaseContext.AddAsync(project);
        }


        #endregion

        public async Task SaveChangesAsync() => await databaseContext.SaveChangesAsync();
    }
}
