using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Ads.Models;
using Ads.Repository;
using Ads.Controllers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Ads.Consumers;
using AutoMapper;
using Ads.DataTransferObjects;

namespace Ads
{
    public static class ConfigureServices
    {
        public static void AddAdsServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connection = configuration.GetConnectionString("DBConnection");
            services.AddDbContext<AdsDBContext>(options => options.UseSqlServer(connection));

            services.AddTransient<IAdsRepository, AdsRepository>();

            services.AddAutoMapper(config =>
            {
                config.CreateMap<Ad, AdDto>();
            });
        }

        public static void AddAdsApplicationParts(this ApplicationPartManager applicationPartManager)
        {
            Assembly assembly = typeof(AdsController).GetTypeInfo().Assembly;
            AssemblyPart part = new AssemblyPart(assembly);
            applicationPartManager.ApplicationParts.Add(part);
        }
        public static void AddAdsMediatorConsumers(this IServiceCollectionMediatorConfigurator configuration)
        {
            configuration.AddConsumer<FileDeletedConsumer>();
        }
    }
}
