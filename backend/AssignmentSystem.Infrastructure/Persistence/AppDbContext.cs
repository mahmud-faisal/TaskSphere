using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ClassCourse> ClassCourses => Set<ClassCourse>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments => Set<TeacherSubjectAssignment>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).HasConversion<string>().IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
        });

        // ClassCourse Configuration
        modelBuilder.Entity<ClassCourse>(entity =>
        {
            entity.ToTable("ClassCourses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // Subject Configuration
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.ClassCourse)
                  .WithMany(c => c.Subjects)
                  .HasForeignKey(e => e.ClassCourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TeacherSubjectAssignment Configuration
        modelBuilder.Entity<TeacherSubjectAssignment>(entity =>
        {
            entity.ToTable("TeacherSubjectAssignments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TeacherId, e.SubjectId, e.ClassCourseId }).IsUnique();

            entity.HasOne(e => e.Teacher)
                  .WithMany()
                  .HasForeignKey(e => e.TeacherId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Subject)
                  .WithMany()
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ClassCourse)
                  .WithMany(c => c.TeacherAssignments)
                  .HasForeignKey(e => e.ClassCourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // StudentEnrollment Configuration
        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.ToTable("StudentEnrollments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StudentId, e.ClassCourseId }).IsUnique();

            entity.HasOne(e => e.Student)
                  .WithMany()
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ClassCourse)
                  .WithMany(c => c.StudentEnrollments)
                  .HasForeignKey(e => e.ClassCourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Assignment Configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            entity.Property(e => e.Deadline).HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(e => e.Subject)
                  .WithMany()
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ClassCourse)
                  .WithMany()
                  .HasForeignKey(e => e.ClassCourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Teacher)
                  .WithMany()
                  .HasForeignKey(e => e.TeacherId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Submission Configuration
        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("Submissions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();
            entity.Property(e => e.Status).HasConversion<string>().IsRequired().HasMaxLength(30);
            entity.Property(e => e.SubmittedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.GradedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(e => e.Assignment)
                  .WithMany(a => a.Submissions)
                  .HasForeignKey(e => e.AssignmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Student)
                  .WithMany()
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.GradedByTeacher)
                  .WithMany()
                  .HasForeignKey(e => e.GradedByTeacherId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
