using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    public class User
    {
        public long Id { get; set; }

        [Required, StringLength(120)]
        [Display(Name = "Full name")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Student;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigations ───────────────────────────────────────────────────────
        public StudentProfile?    StudentProfile   { get; set; }
        public StudentAnalytics?  StudentAnalytics { get; set; }

        public ICollection<Enrollment>           Enrollments     { get; set; } = new List<Enrollment>();
        public ICollection<CourseRequest>        CourseRequests  { get; set; } = new List<CourseRequest>();
        public ICollection<Recommendation>       Recommendations { get; set; } = new List<Recommendation>();
        public ICollection<StudentCourseProgress> Progresses     { get; set; } = new List<StudentCourseProgress>();
        public ICollection<CourseValidationLog>  ReviewLogs      { get; set; } = new List<CourseValidationLog>();
    }
}
