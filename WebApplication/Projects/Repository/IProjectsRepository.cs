using Projects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projects.Repository
{
    public interface IProjectsRepository
    {
        Task<IEnumerable<Project>> GetProjectsAsync();
        Task<Project> GetProjectAsync(int projectId);
        Task<Project> GetProjectAsync(string projectExternalId);
        Task<IEnumerable<Project>> GetUserProjectsAsync(string userExternalId);

        Task<bool> ProjectExists(string projectExternalId);

        Task InsertProjectAsync(Project project);

        void UpdateProject(Project project);

        void DeleteProject(Project project);
        Task DeleteProjectAsync(int projectId);
        Task DeleteProjectAsync(string projectExternalId);
        Task SaveChangesAsync();
    }
}
