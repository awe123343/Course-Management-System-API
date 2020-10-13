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

            modelBuilder.Entity<Course>()
                .Property(b => b.CourseCode)
                .IsRequired();
            modelBuilder.Entity<Course>()
                .Property(b => b.CourseName)
                .IsRequired();
            modelBuilder.Entity<Course>()
                .Property(b => b.Capacity)
                .IsRequired();
            modelBuilder.Entity<Course>()
                .Property(b => b.EvaluatorId)
                .IsRequired();
            modelBuilder.Entity<Course>()
                .HasIndex(b => b.CourseCode)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .Property(b => b.StudentId)
                .IsRequired();
            modelBuilder.Entity<Enrollment>()
                .Property(b => b.CourseId)
                .IsRequired();
            modelBuilder.Entity<Enrollment>()
                .HasIndex(b => new { b.StudentId, b.CourseId })
                .IsUnique();

            modelBuilder.Entity<Family>()
                .Property(b => b.StudentId)
                .IsRequired();
            modelBuilder.Entity<Family>()
                .Property(b => b.ParentId)
                .IsRequired();
            modelBuilder.Entity<Family>()
                .HasIndex(b => new { b.StudentId, b.ParentId })
                .IsUnique();

            modelBuilder.Entity<CourseMaterial>()
                .Property(b => b.CourseId)
                .IsRequired();
            modelBuilder.Entity<CourseMaterial>()
                .Property(b => b.Title)
                .IsRequired();
            modelBuilder.Entity<CourseMaterial>()
                .Property(b => b.Content)
                .IsRequired();
            modelBuilder.Entity<CourseMaterial>()
                .Property(b => b.IsAssignment)
                .IsRequired();

            modelBuilder.Entity<Assignment>()
                .Property(b => b.StudentId)
                .IsRequired();
            modelBuilder.Entity<Assignment>()
                .Property(b => b.CourseMaterialId)
                .IsRequired();
            modelBuilder.Entity<Assignment>()
                .Property(b => b.Submission)
                .IsRequired();
            modelBuilder.Entity<Assignment>()
                .HasIndex(b => new { b.StudentId, b.CourseMaterialId })
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
    }
}