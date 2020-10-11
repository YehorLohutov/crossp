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
            //modelBuilder.Entity<Ad>()
            //    .HasOne(ad => ad.Project)
            //    .WithMany(project => project.Ads)
            //    .HasForeignKey(key => key.ProjectId);

            //Project[] projects = new Project[3]
            //{
            //    new Project {  Id = -1, Name="Project1" },
            //    new Project {  Name="Project2" },
            //    new Project {  Name="Project3" }
            //};
            //modelBuilder.Entity<Project>().HasData(projects);


            //Ad[] ads = new Ad[]
            //{
            //    new Ad() { Id = 1, Url = "url1", ProjectId = 1 },
            //    new Ad() { Id = 2, Url = "url2", ProjectId = 1 },
            //    new Ad() { Id = 3, Url = "url3", ProjectId = 2 },
            //};

            //modelBuilder.Entity<Ad>().HasData(ads);


        }
    }
}
