using Microsoft.EntityFrameworkCore;
using BlazorApi.Models;

namespace BlazorApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Jobs> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Call base to avoid overriding other configurations
            modelBuilder.Entity<Jobs>().HasQueryFilter(j => !j.IsDeleted);

            // Explicitly configure primary key for Jobs
            modelBuilder.Entity<Jobs>().HasKey(j => j.JobId);

            // Optionally, configure additional properties
            modelBuilder.Entity<Jobs>()
                .Property(j => j.JobName)
                .IsRequired()
                .HasMaxLength(100);  // Set max length for JobName to 100 characters
        }
    }
}
