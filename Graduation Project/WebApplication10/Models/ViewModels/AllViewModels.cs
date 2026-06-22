using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models.ViewModels
{
    // ── Student Dashboard ─────────────────────────────────────────────────────
    public class StudentDashboardViewModel
    {
        public User   User                  { get; set; } = null!;
        public int    EnrolledCoursesCount  { get; set; }
        public int    InProgressCount       { get; set; }
        public int    CompletedCount        { get; set; }
        public int    OverallProgressPercent { get; set; }

        public List<EnrollmentCard> ContinueLearning { get; set; } = new();
        public List<TrackCard>      MyTracks          { get; set; } = new();
        public List<Track>          RecommendedTracks { get; set; } = new();

        public StudentProfile? Profile { get; set; }
    }

    public class EnrollmentCard
    {
        public Enrollment Enrollment { get; set; } = null!;
        public Course     Course     { get; set; } = null!;
        public Track?     Track      { get; set; }
    }

    public class TrackCard
    {
        public Track Track           { get; set; } = null!;
        public int   CoursesCount    { get; set; }
        public int   ProgressPercent { get; set; }
    }

    // ── Initiative Detail ─────────────────────────────────────────────────────
    public class InitiativeDetailViewModel
    {
        public Initiative            Initiative { get; set; } = null!;
        public List<TrackDetailItem> Tracks     { get; set; } = new();
    }

    public class TrackDetailItem
    {
        public Track Track       { get; set; } = null!;
        public int   CourseCount { get; set; }
        public bool  IsEnrolled  { get; set; }
    }

    // ── Track Courses ─────────────────────────────────────────────────────────
    public class TrackCoursesViewModel
    {
        public Track                     Track   { get; set; } = null!;
        public List<CourseEnrollmentPair> Courses { get; set; } = new();
    }

    // ── CourseEnrollmentPair ──────────────────────────────────────────────────
    
    public class CourseEnrollmentPair
    {
        public Course     Course     { get; set; } = null!;
        public Enrollment Enrollment { get; set; } = null!;
    }

    // ── Contributor ───────────────────────────────────────────────────────────
    public class ContributorDashboardViewModel
    {
        public int                 PendingCount  { get; set; }
        public int                 ApprovedCount { get; set; }
        public int                 RejectedCount { get; set; }
        public List<CourseRequest> Requests      { get; set; } = new();
    }

    // ── Admin Overview ────────────────────────────────────────────────────────
    public class AdminOverviewViewModel
    {
        public int InitiativesCount     { get; set; }
        public int TracksCount          { get; set; }
        public int CoursesCount         { get; set; }
        public int PendingRequestsCount { get; set; }
        public int OpportunitiesCount   { get; set; }
        public int UsersCount           { get; set; }
    }

    // ── Login form ────────────────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email    { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ── Progress update (AJAX) ────────────────────────────────────────────────
    public class ProgressUpdateRequest
    {
        public long EnrollmentId { get; set; }
        public int  Progress     { get; set; }
    }

    // ── Student Profile ───────────────────────────────────────────────────────
    /// <summary>
    /// Used for both viewing and editing a student profile.
    /// All fields are optional so partial saves work correctly.
    /// </summary>
    public class StudentProfileViewModel
    {
        public long Id     { get; set; }
        public long UserId { get; set; }

        // ── Personal ─────────────────────────────────────────────────────────
        [Display(Name = "Full Name")]
        [StringLength(120, ErrorMessage = "Full name cannot exceed 120 characters.")]
        public string? FullName { get; set; }

        [Display(Name = "Bio / About Me")]
        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
        [DataType(DataType.MultilineText)]
        public string? Bio { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address")]
        [StringLength(300)]
        public string? Address { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        // ── Academic ─────────────────────────────────────────────────────────
        [Display(Name = "University")]
        [StringLength(200)]
        public string? University { get; set; }

        [Display(Name = "Faculty / Department")]
        [StringLength(200)]
        public string? Faculty { get; set; }

        [Display(Name = "Academic Year")]
        [StringLength(50)]
        public string? AcademicYear { get; set; }

        [Display(Name = "GPA")]
        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0.")]
        public double? GPA { get; set; }

        // ── Skills ────────────────────────────────────────────────────────────
        [Display(Name = "Skills (comma-separated)")]
        [StringLength(1000, ErrorMessage = "Skills cannot exceed 1000 characters.")]
        public string? Skills { get; set; }

        // ── Links ─────────────────────────────────────────────────────────────
        [Display(Name = "LinkedIn URL")]
        [Url(ErrorMessage = "Enter a valid URL starting with https://")]
        [StringLength(300)]
        public string? LinkedInUrl { get; set; }

        [Display(Name = "GitHub URL")]
        [Url(ErrorMessage = "Enter a valid URL starting with https://")]
        [StringLength(300)]
        public string? GithubUrl { get; set; }

        [Display(Name = "Portfolio URL")]
        [Url(ErrorMessage = "Enter a valid URL starting with https://")]
        [StringLength(300)]
        public string? PortfolioUrl { get; set; }

        // ── Uploaded files (display-only — uploads handled via IFormFile) ─────
        public string? CVUrl           { get; set; }
        public string? ProfileImageUrl { get; set; }

        // ── Computed display helpers ──────────────────────────────────────────
        public string SafeProfileImage =>
            string.IsNullOrEmpty(ProfileImageUrl)
                ? "/images/default-avatar.png"
                : ProfileImageUrl;

        public IEnumerable<string> SkillList =>
            string.IsNullOrWhiteSpace(Skills)
                ? Enumerable.Empty<string>()
                : Skills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0);

        // ── File upload inputs (not persisted directly) ───────────────────────
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImageFile { get; set; }

        [Display(Name = "CV / Resume (PDF)")]
        public IFormFile? CVFile { get; set; }

        // ── Read-only user info shown on the profile page ─────────────────────
        public string UserName  { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
