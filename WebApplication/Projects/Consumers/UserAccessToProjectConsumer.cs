using Projects.Contracts;
using Projects.Repository;
using Projects.Models;
using System.Threading.Tasks;
using MassTransit;
using System;

namespace Projects.Consumers
{
    public class UserAccessToProjectConsumer : IConsumer<GetUserAccessToProjectRequest>
    {
        private readonly IProjectsRepository projectsRepository;
        public UserAccessToProjectConsumer(IProjectsRepository projectsRepository) =>
            this.projectsRepository = projectsRepository;

        public async Task Consume(ConsumeContext<GetUserAccessToProjectRequest> context)
        {
            Project project = default;
            try
            {
                project = await projectsRepository.GetProjectAsync(context.Message.ProjectExternalId);
            }
            catch (Exception)
            {
                await context.RespondAsync(new UserAccessToProjectResponse(context.Message.UserExternalId, context.Message.ProjectExternalId, false));
            }

            if (!project.UserExternalId.Equals(context.Message.UserExternalId))
                await context.RespondAsync(new UserAccessToProjectResponse(context.Message.UserExternalId, context.Message.ProjectExternalId, false));

            await context.RespondAsync(new UserAccessToProjectResponse(context.Message.UserExternalId, context.Message.ProjectExternalId, true));
        }
    }
}
