// =============================================================================
//  AppDbContext.cs

using Microsoft.EntityFrameworkCore;

namespace PathwayPlatform.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // ── DbSets ────────────────────────────────────────────────────────────
        public DbSet<User>                   Users                   { get; set; } = null!;
        public DbSet<StudentProfile>         StudentProfiles         { get; set; } = null!;
        public DbSet<StudentAnalytics>       StudentAnalytics        { get; set; } = null!;
        public DbSet<Initiative>             Initiatives             { get; set; } = null!;
        public DbSet<Track>                  Tracks                  { get; set; } = null!;
        public DbSet<Course>                 Courses                 { get; set; } = null!;
        //public DbSet<CourseItem>             CourseItems             { get; set; } = null!;
        public DbSet<Enrollment>             Enrollments             { get; set; } = null!;
        public DbSet<StudentCourseProgress>  StudentCourseProgresses { get; set; } = null!;
        public DbSet<Recommendation>         Recommendations         { get; set; } = null!;
        public DbSet<CourseRequest>          CourseRequests          { get; set; } = null!;
        public DbSet<CourseValidationLog>    CourseValidationLogs    { get; set; } = null!;
        public DbSet<Opportunity>            Opportunities           { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // ── StudentProfile ────────────────────────────────────────────────
            mb.Entity<StudentProfile>(b =>
            {
                b.HasOne(sp => sp.User)
                 .WithOne(u => u.StudentProfile)
                 .HasForeignKey<StudentProfile>(sp => sp.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Ignore computed-property aliases — EF must not try to map them
                b.Ignore(sp => sp.LinkedInLink);
                b.Ignore(sp => sp.GithubLink);
                // Ignore computed helpers (not columns)
                b.Ignore(sp => sp.SafeProfileImage);
                b.Ignore(sp => sp.SkillList);

                b.Property(sp => sp.FullName).HasMaxLength(120);
                b.Property(sp => sp.Bio).HasMaxLength(1000);
                b.Property(sp => sp.PhoneNumber).HasMaxLength(30);
                b.Property(sp => sp.Address).HasMaxLength(300);
                b.Property(sp => sp.Gender).HasMaxLength(20);
                b.Property(sp => sp.University).HasMaxLength(200);
                b.Property(sp => sp.Faculty).HasMaxLength(200);
                b.Property(sp => sp.Major).HasMaxLength(120);
                b.Property(sp => sp.AcademicYear).HasMaxLength(50);
                b.Property(sp => sp.Level).HasMaxLength(50);
                b.Property(sp => sp.Skills).HasMaxLength(1000);
                b.Property(sp => sp.LinkedInUrl).HasMaxLength(300);
                b.Property(sp => sp.GithubUrl).HasMaxLength(300);
                b.Property(sp => sp.PortfolioUrl).HasMaxLength(300);
                b.Property(sp => sp.CVUrl).HasMaxLength(500);
                b.Property(sp => sp.ProfileImageUrl).HasMaxLength(500);
            });

            // ── StudentAnalytics ──────────────────────────────────────────────
            mb.Entity<User>()
              .HasOne(u => u.StudentAnalytics)
              .WithOne(sa => sa.User)
              .HasForeignKey<StudentAnalytics>(sa => sa.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            // ── User → collections ────────────────────────────────────────────
            mb.Entity<User>()
              .HasMany(u => u.Enrollments)
              .WithOne(e => e.User)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
              .HasMany(u => u.Progresses)
              .WithOne(p => p.User)
              .HasForeignKey(p => p.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
              .HasMany(u => u.Recommendations)
              .WithOne(r => r.User)
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
              .HasMany(u => u.CourseRequests)
              .WithOne(cr => cr.User)
              .HasForeignKey(cr => cr.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
              .HasMany(u => u.ReviewLogs)
              .WithOne(v => v.Reviewer)
              .HasForeignKey(v => v.ReviewerId)
              .OnDelete(DeleteBehavior.Restrict);

            // ── Initiative → Track ────────────────────────────────────────────
            mb.Entity<Initiative>()
              .HasMany(i => i.Tracks)
              .WithOne(t => t.Initiative)
              .HasForeignKey(t => t.InitiativeId)
              .OnDelete(DeleteBehavior.Cascade);

            // ── Track → Course ────────────────────────────────────────────────
            mb.Entity<Track>()
              .HasMany(t => t.Courses)
              .WithOne(c => c.Track)
              .HasForeignKey(c => c.TrackId)
              .OnDelete(DeleteBehavior.Cascade);

         
     

            // ── Course → Enrollments ──────────────────────────────────────────
            mb.Entity<Course>()
              .HasMany(c => c.Enrollments)
              .WithOne(e => e.Course)
              .HasForeignKey(e => e.CourseId)
              .OnDelete(DeleteBehavior.Cascade);

            // ── Course → Recommendations ──────────────────────────────────────
            mb.Entity<Course>()
              .HasMany(c => c.Recommendations)
              .WithOne(r => r.Course)
              .HasForeignKey(r => r.CourseId)
              .OnDelete(DeleteBehavior.Cascade);

          

            // ── CourseRequest relations ───────────────────────────────────────
            mb.Entity<CourseRequest>()
              .HasOne(cr => cr.Track)
              .WithMany(t => t.CourseRequests)
              .HasForeignKey(cr => cr.TrackId)
              .OnDelete(DeleteBehavior.NoAction);

            mb.Entity<CourseRequest>()
              .HasOne(cr => cr.GeneratedCourse)
              .WithMany(c => c.GeneratedForRequests)
              .HasForeignKey(cr => cr.GeneratedCourseId)
              .OnDelete(DeleteBehavior.NoAction);

   

            mb.Entity<CourseRequest>()
              .HasMany(cr => cr.ValidationLogs)
              .WithOne(v => v.CourseRequest)
              .HasForeignKey(v => v.CourseRequestId)
              .OnDelete(DeleteBehavior.Cascade);

            // ── Indexes ───────────────────────────────────────────────────────
            mb.Entity<User>()
              .HasIndex(u => u.Email)
              .IsUnique();

            mb.Entity<Enrollment>()
              .HasIndex(e => new { e.UserId, e.CourseId })
              .IsUnique();

            mb.Entity<StudentCourseProgress>()
              .HasIndex(p => new { p.UserId, p.CourseItemId })
              .IsUnique();

            mb.Entity<Recommendation>()
              .HasIndex(r => new { r.UserId, r.CourseId });
        }
    }
}
