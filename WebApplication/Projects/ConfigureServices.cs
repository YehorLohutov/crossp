using Microsoft.EntityFrameworkCore;
using Projects.Models;
using Projects.Repository;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Projects.Consumers;
using Projects.DataTransferObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Projects
{
    public static class ConfigureServices
    {
        public static void AddProjectsServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connection = configuration.GetConnectionString("DBConnection");
            services.AddDbContext<ProjectsDatabaseContext>(options => options.UseSqlServer(connection));

            services.AddTransient<IProjectsRepository, ProjectsRepository>();

            services.AddAutoMapper(config =>
            {
                config.CreateMap<Project, ProjectDto>();
            });
        }

        public static void AddProjectsMediatorConsumers(this IServiceCollectionMediatorConfigurator configuration)
        {
            configuration.AddConsumer<UserAccessToProjectConsumer>();
            configuration.AddConsumer<ProjectStatusConsumer>();
        }
    }
}
