using Microsoft.EntityFrameworkCore;
using BlazorApi.Models;

namespace BlazorApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

     
      

        public DbSet<User> Users { get; set; }
    }
}
