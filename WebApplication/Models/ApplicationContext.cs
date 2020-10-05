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
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasData(
                new Project[]
                {
                new Project { Id=1, Name="Project1" },
                new Project { Id=2, Name="Project2" },
                new Project { Id=3, Name="Project3" }
                });
        }
    }
}
