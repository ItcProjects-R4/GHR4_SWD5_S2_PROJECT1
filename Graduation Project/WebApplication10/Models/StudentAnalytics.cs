// =============================================================================
//  StudentAnalytics.cs
// =============================================================================
namespace PathwayPlatform.Models
{
    public class StudentAnalytics
    {
        public long    Id                    { get; set; }
        public long    UserId                { get; set; }
        public int     TotalCoursesCompleted { get; set; }
        public decimal CompletionRate        { get; set; }
        public string? StrengthAreas         { get; set; }
        public string? WeaknessAreas         { get; set; }
        public DateTime UpdatedAt            { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
