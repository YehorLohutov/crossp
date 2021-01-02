using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;


namespace WebApplication.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }

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

            //modelBuilder.Entity<Storage>().HasOne(storage => storage.User);
            //modelBuilder.Entity<Storage>().HasOne(storage => storage.File);

            Project[] projects = new Project[3]
            {
                new Project {  Id = 1, Name="Project1" },
                new Project {  Id = 2, Name="Project2" },
                new Project {  Id = 3, Name="Project3" }
            };
            modelBuilder.Entity<Project>().HasData(projects);

            User[] users = new User[]
            {
                new User() { Id = 1, Login = "user1", Password = "user1" },
                new User() { Id = 2, Login = "user2", Password = "user2" }
            };

            modelBuilder.Entity<User>().HasData(users);

            File[] files = new File[]
            {
                new File() {Id = 1, Path = "/Storages/1/image1.jpg", UserId = 1 },
                new File() {Id = 2, Path = "/Storages/2/image2.jpg", UserId = 2 }
            };
            foreach (File file in files)
            {
                file.Name = Path.GetFileNameWithoutExtension(file.Path);
                file.Extension = Path.GetExtension(file.Path);
            }

            modelBuilder.Entity<File>().HasData(files);

            Ad[] ads = new Ad[]
{
                new Ad() { Id = 1, Name = "Ad1",  Url = "url1", ProjectId = 1, FileId = 1 },
                new Ad() { Id = 2, Name = "Ad2", Url = "url2", ProjectId = 2, FileId = 2 },
                new Ad() { Id = 3, Name = "Ad3", Url = "url3", ProjectId = 3, FileId = 1 },
};

            modelBuilder.Entity<Ad>().HasData(ads);
        }

        //public async Models.File GetDefaultImageFile(User )
        //{
        //    Models.File defaultImageFile = await Files.FirstOrDefaultAsync(file => file.Path == "/default.png");
            
        //    if(defaultImageFile is null)
        //    {
        //        defaultImageFile = new File() { Name = }
        //    }
        //    return defaultImageFile;
        //}
    }
}
