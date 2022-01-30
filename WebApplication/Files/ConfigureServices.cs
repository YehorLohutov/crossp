using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Files.Models;
using Microsoft.Extensions.Configuration;
using Files.Repository;
using Files.Controller;
using Files.MessagesSubscribers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Files.Consumers;
using Files.DataTransferObjects;
using AutoMapper;

namespace Files
{
    public static class ConfigureServices
    {
        public static void AddFilesServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connection = configuration.GetConnectionString("DBConnection");
            services.AddDbContext<FilesDatabaseContext>(options => options.UseSqlServer(connection));

            services.AddTransient<IFilesRepository, FilesRepository>();

            services.AddAutoMapper(config =>
            {
                config.CreateMap<Models.File, FileDto>();
            });
        }

        public static void AddFilesMediatorConsumers(this IServiceCollectionMediatorConfigurator configuration)
        {
            configuration.AddConsumer<DefaultFileExternalIdConsumer>();
        }

    }
}
