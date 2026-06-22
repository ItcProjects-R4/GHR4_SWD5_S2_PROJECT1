using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    public class Course
    {
        public long Id      { get; set; }
        public long TrackId { get; set; }

        [Required, StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ContentUrl { get; set; }

        [StringLength(100)]
        public string? Duration { get; set; }

        [StringLength(100)]
        public string? SourceType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Track Track { get; set; } = null!;

       
        public ICollection<Enrollment>    Enrollments           { get; set; } = new List<Enrollment>();
        public ICollection<Recommendation> Recommendations      { get; set; } = new List<Recommendation>();
        public ICollection<CourseRequest> GeneratedForRequests  { get; set; } = new List<CourseRequest>();
    }
}
