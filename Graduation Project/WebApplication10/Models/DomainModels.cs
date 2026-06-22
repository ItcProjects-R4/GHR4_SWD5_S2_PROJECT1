using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    

    // ── Enrollment ────────────────────────────────────────────────────────────
    public class Enrollment
    {
        public long Id       { get; set; }
        public long UserId   { get; set; }
        public long CourseId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "not-started";

        public decimal ProgressPercentage { get; set; } = 0;

        public User   User   { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    // ── StudentCourseProgress ─────────────────────────────────────────────────
    public class StudentCourseProgress
    {
        public long Id           { get; set; }
        public long UserId       { get; set; }
        public long CourseItemId { get; set; }

        public bool      IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public User       User       { get; set; } = null!;
        
    }

    // ── Recommendation ────────────────────────────────────────────────────────
    public class Recommendation
    {
        public long Id       { get; set; }
        public long UserId   { get; set; }
        public long CourseId { get; set; }

        [StringLength(100)]
        public string? Type { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        public decimal  Score     { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User   User   { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

    // ── CourseRequest ─────────────────────────────────────────────────────────
    public class CourseRequest
    {
        public long  Id                { get; set; }
        public long  UserId            { get; set; }
        public long? TrackId           { get; set; }
        public long? GeneratedCourseId { get; set; }

        [Required, StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ContentUrl { get; set; }

        [StringLength(100)]
        public string? Duration { get; set; }

        [StringLength(200)]
        public string? ProviderName { get; set; }

        [StringLength(200)]
        public string? ProviderEmail { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User    User            { get; set; } = null!;
        public Track?  Track           { get; set; }
        public Course? GeneratedCourse { get; set; }

        public ICollection<CourseValidationLog> ValidationLogs { get; set; } = new List<CourseValidationLog>();
    }

    // ── CourseValidationLog ───────────────────────────────────────────────────
    public class CourseValidationLog
    {
        public long Id              { get; set; }
        public long CourseRequestId { get; set; }
        public long ReviewerId      { get; set; }

        [StringLength(50)]
        public string? Decision { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        public CourseRequest CourseRequest { get; set; } = null!;
        public User          Reviewer      { get; set; } = null!;
    }

    // ── Opportunity ───────────────────────────────────────────────────────────
    public class Opportunity
    {
        public long Id { get; set; }

        [Required, StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? RequiredSkills { get; set; }

        [StringLength(500)]
        public string? Link { get; set; }

        [StringLength(200)]
        public string? Source { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ── ErrorViewModel ────────────────────────────────────────────────────────
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
