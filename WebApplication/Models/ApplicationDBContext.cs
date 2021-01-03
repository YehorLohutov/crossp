using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication.Models;


namespace WebApplication.Models
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAccess> ProjectAccesses { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileAccess> FileAccesses { get; set; }

        protected const string DEFAULT_PNG_FILE_PATH = "/default.png";
        protected const string DEFAULT_MP4_FILE_PATH = "/default.mp4";

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>(DefaultFilesConfigure);
            
            


            //modelBuilder.Entity<Ad>()
            //    .HasOne(ad => ad.Project)
            //    .WithMany(project => project.Ads)
            //    .HasForeignKey(key => key.ProjectId);

            //modelBuilder.Entity<Storage>().HasOne(storage => storage.User);
            //modelBuilder.Entity<Storage>().HasOne(storage => storage.File);

//            Project[] projects = new Project[3]
//            {
//                new Project {  Id = 1, Name="Project1" },
//                new Project {  Id = 2, Name="Project2" },
//                new Project {  Id = 3, Name="Project3" }
//            };
//            modelBuilder.Entity<Project>().HasData(projects);

//            User[] users = new User[]
//            {
//                new User() { Id = 1, Login = "user1", Password = "user1" },
//                new User() { Id = 2, Login = "user2", Password = "user2" }
//            };

//            modelBuilder.Entity<User>().HasData(users);

//            File[] files = new File[]
//            {
//                new File() {Id = 1, Path = "/Storages/1/image1.jpg" },
//                new File() {Id = 2, Path = "/Storages/2/image2.jpg" }
//            };
//            foreach (File file in files)
//            {
//                file.Name = Path.GetFileNameWithoutExtension(file.Path);
//                file.Extension = Path.GetExtension(file.Path);
//            }

//            modelBuilder.Entity<File>().HasData(files);

//            Ad[] ads = new Ad[]
//{
//                new Ad() { Id = 1, Name = "Ad1",  Url = "url1", ProjectId = 1, FileId = 1 },
//                new Ad() { Id = 2, Name = "Ad2", Url = "url2", ProjectId = 2, FileId = 2 },
//                new Ad() { Id = 3, Name = "Ad3", Url = "url3", ProjectId = 3, FileId = 1 },
//};

//            modelBuilder.Entity<Ad>().HasData(ads);
        }



        protected void DefaultFilesConfigure(EntityTypeBuilder<File> builder)
        {
            File[] files = new File[]
            {
                new File() { Id = 1, Path = DEFAULT_PNG_FILE_PATH },
                new File() { Id = 2, Path = DEFAULT_MP4_FILE_PATH }
            };
            foreach (File file in files)
            {
                file.Name = Path.GetFileNameWithoutExtension(file.Path);
                file.Extension = Path.GetExtension(file.Path);
            }
            builder.HasData(files);
        }

        protected void AdminConfigure(ModelBuilder modelBuilder)
        {
            User admin = CreateNewUser("admin", "admin");

            Project testProject = new Project() { Name = "Test Project" };
            Projects.Add(testProject);

            Ad testAd = new Ad() { Name = "Test Ad", Url = "https://www.google.com/", ProjectId = testProject.Id, FileId = GetDefaultPNGFile().Id };
            Ads.Add(testAd);

            ProjectAccess projectAccess = new ProjectAccess() { AccessType = ProjectAccess.ProjectAccessType.Owner, ProjectId = testProject.Id, UserId = admin.Id };
            ProjectAccesses.Add(projectAccess);


        }

        public async Task<Models.File> GetDefaultPNGFileAsync() => 
            await Files.FirstAsync(file => file.Path.Equals(DEFAULT_PNG_FILE_PATH));

        public Models.File GetDefaultPNGFile() =>
            Files.First(file => file.Path.Equals(DEFAULT_PNG_FILE_PATH));

        public async Task<Models.File> GetDefaultMP4FileAsync() =>
            await Files.FirstAsync(file => file.Path.Equals(DEFAULT_MP4_FILE_PATH));

        public Models.File GetDefaultMP4File() =>
            Files.First(file => file.Path.Equals(DEFAULT_MP4_FILE_PATH));

        public async Task<User> CreateNewUserAsync(string login, string password)
        {
            User newUser = new User() { Login = login, Password = password };
            await Users.AddAsync(newUser);
          
            Models.File defaultPNGFile = await GetDefaultPNGFileAsync();
            Models.File defaultMP4File = await GetDefaultMP4FileAsync();

            FileAccess defaultPNGFileAccess = new FileAccess() { AccessType = FileAccess.FileAccessType.Reader, UserId = newUser.Id, FileId = defaultPNGFile.Id };
            FileAccess defaultMP4FileAccess = new FileAccess() { AccessType = FileAccess.FileAccessType.Reader, UserId = newUser.Id, FileId = defaultMP4File.Id };
            await FileAccesses.AddRangeAsync(defaultPNGFileAccess, defaultMP4FileAccess);

            await this.SaveChangesAsync();
            return newUser;
        }

        public User CreateNewUser(string login, string password)
        {
            User newUser = new User() { Login = login, Password = password };
            Users.Add(newUser);

            Models.File defaultPNGFile = GetDefaultPNGFile();
            Models.File defaultMP4File = GetDefaultMP4File();

            FileAccess defaultPNGFileAccess = new FileAccess() { AccessType = FileAccess.FileAccessType.Reader, UserId = newUser.Id, FileId = defaultPNGFile.Id };
            FileAccess defaultMP4FileAccess = new FileAccess() { AccessType = FileAccess.FileAccessType.Reader, UserId = newUser.Id, FileId = defaultMP4File.Id };
            FileAccesses.AddRange(defaultPNGFileAccess, defaultMP4FileAccess);

            this.SaveChangesAsync();
            return newUser;
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

    //public class ProductConfiguration : IEntityTypeConfiguration<Models.File>
    //{
    //    public void Configure(EntityTypeBuilder<Models.File> builder)
    //    {
    //    }
    //}
}
