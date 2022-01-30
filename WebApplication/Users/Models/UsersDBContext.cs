using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Users.Models
{
    public class UsersDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly IConfiguration configuration;
        public UsersDBContext(DbContextOptions<UsersDBContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
            //var comparer = new CompareEfSql();
            //comparer.CompareEfWithDb(
            //ATTEMPT
            //This will use the database provider design time type you gave to get the database information
            //var hasErrors = comparer.CompareEfWithDb<NpgsqlDesignTimeServices>(context));
            
            
            //IRelationalDatabaseCreator databaseCreator = this.GetService<IRelationalDatabaseCreator>();

            ////this.Database.

            //try
            //{
            //    if (!databaseCreator.Exists())
            //        databaseCreator.Create();

            //    databaseCreator.CreateTables();
            //} 
            //catch (Exception)
            //{

            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(nameof(UsersDBContext));

            IConfigurationSection defaultUsersConfigurationSection = configuration.GetSection("DefaultUsers");
            IConfigurationSection[] configUsers = defaultUsersConfigurationSection.GetChildren().ToArray();

            List<User> users = new List<User>();
            for (int i = 0; i < configUsers.Length; i++)
            {
                int id = int.Parse(configUsers[i].GetSection("Id").Value);
                string login = configUsers[i].GetSection("Login").Value;
                string password = configUsers[i].GetSection("Password").Value;
                string externalId = configUsers[i].GetSection("ExternalId").Value;

                users.Add(new User(id, login, password, externalId));
            }

            modelBuilder.Entity<User>().HasData(users);
        }
    }
}
