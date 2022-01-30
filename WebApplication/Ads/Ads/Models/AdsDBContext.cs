using Microsoft.EntityFrameworkCore;

namespace Ads.Models
{
    public class AdsDBContext : DbContext
    {
        public DbSet<Ad> Ads { get; set; }
        public DbSet<AdClicksStats> AdClicksStats { get; set; }
        public DbSet<AdShowStats> AdShowStats { get; set; }

        public AdsDBContext(DbContextOptions<AdsDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(nameof(AdsDBContext));
        }
    }
}
