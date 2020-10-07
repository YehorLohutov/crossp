using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace WebApplication.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ad> Ads { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Ad ad = new Ad() { Id = 1, Url = "url1" };
            modelBuilder.Entity<Ad>().HasData(ad);

            Project[] projects = new Project[3]
            {
                new Project {  Id = 1, Name="Project1", Ads=new List<Ad>(){ ad } },
                new Project {  Id = 2, Name="Project2" },
                new Project {  Id = 3, Name="Project3" }
            };
            modelBuilder.Entity<Project>().HasData(projects);
        }
    }
}
