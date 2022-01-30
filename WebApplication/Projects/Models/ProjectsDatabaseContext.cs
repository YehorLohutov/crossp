using Microsoft.EntityFrameworkCore;

namespace Projects.Models
{
    public class ProjectsDatabaseContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public ProjectsDatabaseContext(DbContextOptions<ProjectsDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(nameof(ProjectsDatabaseContext));
        }
    }
}
