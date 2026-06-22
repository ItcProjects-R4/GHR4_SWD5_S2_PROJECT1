
using System.ComponentModel.DataAnnotations;

namespace PathwayPlatform.Models
{
    public class StudentProfile
    {
        public long Id     { get; set; }
        public long UserId { get; set; }

        // ── Personal ──────────────────────────────────────────────────────────
        [StringLength(120)]
        public string? FullName { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        // ── Academic ──────────────────────────────────────────────────────────
        [StringLength(200)]
        public string? University { get; set; }

        [StringLength(200)]
        public string? Faculty { get; set; }

        /// <summary>Legacy field — maps to Faculty in the BuildViewModel helper.</summary>
        [StringLength(120)]
        public string? Major { get; set; }

        [StringLength(50)]
        public string? AcademicYear { get; set; }

        /// <summary>Legacy alias for AcademicYear.</summary>
        [StringLength(50)]
        public string? Level { get; set; }

        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
        public double? GPA { get; set; }

        // ── Skills (comma-separated, displayed as tags) ───────────────────────
        [StringLength(1000)]
        public string? Skills { get; set; }

        // ── Social & portfolio links ──────────────────────────────────────────
        [Url, StringLength(300)]
        public string? LinkedInUrl { get; set; }

        /// <summary>Legacy alias — not mapped to a DB column (ignored in Fluent API).</summary>
        [StringLength(300)]
        public string? LinkedInLink
        {
            get => LinkedInUrl;
            set => LinkedInUrl = value;
        }

        [Url, StringLength(300)]
        public string? GithubUrl { get; set; }

        /// <summary>Legacy alias — not mapped (ignored in Fluent API).</summary>
        [StringLength(300)]
        public string? GithubLink
        {
            get => GithubUrl;
            set => GithubUrl = value;
        }

        [Url, StringLength(300)]
        public string? PortfolioUrl { get; set; }

        // ── Uploaded file paths (stored relative to wwwroot) ──────────────────
        [StringLength(500)]
        public string? CVUrl { get; set; }

        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        // ── Audit ─────────────────────────────────────────────────────────────
        // FIX: CreatedAt and UpdatedAt are required by StudentProfileController.Edit
        public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ── Navigation ────────────────────────────────────────────────────────
        public User User { get; set; } = null!;

        // ── Computed helpers (not mapped to DB columns) ───────────────────────
        /// <summary>Returns the profile image URL or a default avatar placeholder.</summary>
        public string SafeProfileImage =>
            string.IsNullOrEmpty(ProfileImageUrl)
                ? "/images/default-avatar.png"
                : ProfileImageUrl;

        /// <summary>Splits Skills into a trimmed list for display as tags.</summary>
        public IEnumerable<string> SkillList =>
            string.IsNullOrWhiteSpace(Skills)
                ? Enumerable.Empty<string>()
                : Skills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0);
    }
}
