using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Files.Models
{
    public class FilesDatabaseContext : DbContext
    {
        public DbSet<File> Files { get; set; }

        private readonly IConfiguration configuration = default;

        public FilesDatabaseContext(DbContextOptions<FilesDatabaseContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(nameof(FilesDatabaseContext));

            List<string> defaultUsersExternalIds = GetDefaultUsersExternalIds();

            IConfigurationSection defaultFilesConfigurationSection = configuration.GetSection("DefaultFiles");
            IConfigurationSection[] configFiles = defaultFilesConfigurationSection.GetChildren().ToArray();

            List<File> files = new List<File>();
            for (int i = 0; i < configFiles.Length; i++)
            {
                int fileId = int.Parse(configFiles[i].GetSection("Id").Value);
                string fileName = configFiles[i].Key;
                string filePath = configFiles[i].GetSection("Path").Value;

                File file = new File(fileId, filePath) { Name = fileName, UserExternalId = defaultUsersExternalIds[0] };
                files.Add(file);
                //messageBroker.Publish(nameof(FileCreatedMessage), new FileCreatedMessage(file.ExternalId));
            }

            modelBuilder.Entity<File>().HasData(files);
        }

        private List<string> GetDefaultUsersExternalIds()
        {
            IConfigurationSection defaultUsersConfigurationSection = configuration.GetSection("DefaultUsers");
            IConfigurationSection[] configUsers = defaultUsersConfigurationSection.GetChildren().ToArray();

            List<string> defaultUsersExternalIds = new List<string>();
            for (int i = 0; i < configUsers.Length; i++)
            {
                string externalId = configUsers[i].GetSection("ExternalId").Value;
                defaultUsersExternalIds.Add(externalId);
            }

            return defaultUsersExternalIds;
        }



        public List<int> GetDefaultFilesId()
        {
            IConfigurationSection defaultFilesConfigurationSection = configuration.GetSection("DefaultFiles");
            IConfigurationSection[] configFiles = defaultFilesConfigurationSection.GetChildren().ToArray();

            List<int> defaultFilesId = new List<int>();
            for (int i = 0; i < configFiles.Length; i++)
            {
                int fileId = int.Parse(configFiles[i].GetSection("Id").Value);
                defaultFilesId.Add(fileId);
            }
            return defaultFilesId;
        }

        public async Task<List<File>> GetDefaultFilesAsync()
        {
            List<int> defaultFilesId = GetDefaultFilesId();
            return await Files.Where(file => defaultFilesId.Contains(file.Id)).ToListAsync();
        }
        public bool IsFileDefault(File file) => GetDefaultFilesId().Contains(file.Id);
        
    }
}
