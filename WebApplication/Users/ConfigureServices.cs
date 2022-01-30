using Microsoft.EntityFrameworkCore;
using Users.Models;
using Users.Repository;
using Users.Consumers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Users
{
    public static class ConfigureServices
    {
        public static void AddUsersServices(this IServiceCollection services, IConfiguration configuration)
        {  
            string connection = configuration.GetConnectionString("DBConnection");
            services.AddDbContext<UsersDBContext>(options => options.UseSqlServer(connection));

            services.AddTransient<IUsersRepository, UsersRepository>();
        }

        public static void AddUsersMediatorConsumers(this IServiceCollectionMediatorConfigurator configuration)
        {
            configuration.AddConsumer<UserStatusConsumer>();
        }
    }
}
