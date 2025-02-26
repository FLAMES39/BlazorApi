using Microsoft.EntityFrameworkCore;
using BlazorApi.Models;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace BlazorApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.IsConfigured("Server=localhost;Database=job_recruitment_db;User=root;Password=SUNPRO100#;Port=3306");
        //}

        public DbSet<User> Users { get; set; }
        public DbSet<Jobs> Jobs { get; set; }

        public DbSet<Applications> Applications { get; set; }



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
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasColumnType("int");
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();



        }
    }
}
