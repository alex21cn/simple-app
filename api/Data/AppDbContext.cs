using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FileReport> FileReports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileReport>(eb =>
            {
                eb.ToTable("FileReport");
                eb.HasKey(e => e.Id).HasName("FileReport_pkey");
                eb.Property(e => e.NumberOfLinkedNodes).HasDefaultValue(0);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
