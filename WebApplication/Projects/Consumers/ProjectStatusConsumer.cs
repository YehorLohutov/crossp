using MassTransit;
using System.Threading.Tasks;
using Projects.Repository;
using Projects.Contracts;

namespace Projects.Consumers
{
    public class ProjectStatusConsumer : IConsumer<GetProjectStatusRequest>
    {
        private readonly IProjectsRepository projectsRepository;
        public ProjectStatusConsumer(IProjectsRepository projectsRepository) =>
            this.projectsRepository = projectsRepository;
        
        public async Task Consume(ConsumeContext<GetProjectStatusRequest> context)
        {
            bool exist = await projectsRepository.ProjectExists(context.Message.ProjectExternalId);
            await context.RespondAsync(new ProjectStatusResponse(context.Message.ProjectExternalId, exist));
        }
    }
}
