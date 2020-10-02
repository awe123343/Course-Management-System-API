using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CourseSysAPI.Entities;

namespace CourseSysAPI.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(b => b.Username)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(b => b.PasswordHash)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(b => b.PasswordSalt)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(b => b.Role)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Username)
                .IsUnique();

        }

        public DbSet<User> Users { get; set; }
    }
}