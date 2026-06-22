using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    public class Track
    {
        public long Id           { get; set; }
        public long InitiativeId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Type { get; set; }

        public Initiative Initiative { get; set; } = null!;

        public ICollection<Course>        Courses        { get; set; } = new List<Course>();
        public ICollection<CourseRequest> CourseRequests { get; set; } = new List<CourseRequest>();
    }
}
