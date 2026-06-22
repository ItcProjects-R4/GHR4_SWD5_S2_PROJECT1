using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;
using PathwayPlatform.Models.ViewModels;

namespace PathwayPlatform.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;

        public StudentController(AppDbContext db) => _db = db;

        private long? Uid => (long?)HttpContext.Session.GetInt32("UserId");
        private IActionResult NeedLogin() => RedirectToAction("Login", "Users");

        // ── Dashboard ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            if (Uid is null) return NeedLogin();

            var enrollments = await _db.Enrollments
                .Where(e => e.UserId == Uid)
                .Include(e => e.Course).ThenInclude(c => c.Track)
                .ToListAsync();

            var completed  = enrollments.Count(e => e.Status == "completed");
            var inProgress = enrollments.Count(e => e.Status == "in-progress");
            var overall    = enrollments.Any()
                ? (int)Math.Round(enrollments.Average(e => (double)e.ProgressPercentage))
                : 0;

            var continueLearning = enrollments
                .Where(e => e.Status != "completed")
                .Take(4)
                .Select(e => new EnrollmentCard
                {
                    Enrollment = e,
                    Course     = e.Course,
                    Track      = e.Course?.Track
                }).ToList();

            var enrolledTrackIds = enrollments
                .Where(e => e.Course is not null)
                .Select(e => e.Course.TrackId)
                .Distinct()
                .ToList();

            var myTracks = new List<TrackCard>();
            foreach (var tid in enrolledTrackIds)
            {
                var track = await _db.Tracks
                    .Include(t => t.Courses)
                    .Include(t => t.Initiative)
                    .FirstOrDefaultAsync(t => t.Id == tid);
                if (track is null) continue;

                var trackEnrollments = enrollments.Where(e => e.Course?.TrackId == tid).ToList();
                var pct = trackEnrollments.Any()
                    ? (int)Math.Round(trackEnrollments.Average(e => (double)e.ProgressPercentage))
                    : 0;

                myTracks.Add(new TrackCard
                {
                    Track           = track,
                    CoursesCount    = track.Courses.Count,
                    ProgressPercent = pct
                });
            }

            var recommended = await _db.Tracks
                .Include(t => t.Initiative)
                .Where(t => !enrolledTrackIds.Contains(t.Id))
                .Take(3)
                .ToListAsync();

            var user    = await _db.Users.FindAsync(Uid) ?? new User();
            var profile = await _db.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == Uid);

            return View(new StudentDashboardViewModel
            {
                User                   = user,
                EnrolledCoursesCount   = enrollments.Count,
                InProgressCount        = inProgress,
                CompletedCount         = completed,
                OverallProgressPercent = overall,
                ContinueLearning       = continueLearning,
                MyTracks               = myTracks,
                RecommendedTracks      = recommended,
                Profile                = profile
            });
        }

        // ── Initiatives ───────────────────────────────────────────────────────
        public async Task<IActionResult> Initiatives()
        {
            if (Uid is null) return NeedLogin();
            var list = await _db.Initiatives
                .Include(i => i.Tracks)
                .OrderBy(i => i.Name)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> InitiativeDetail(long id)
        {
            if (Uid is null) return NeedLogin();

            var initiative = await _db.Initiatives
                .Include(i => i.Tracks).ThenInclude(t => t.Courses)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (initiative is null) return NotFound();

            var enrolledTrackIds = await _db.Enrollments
                .Where(e => e.UserId == Uid)
                .Include(e => e.Course)
                .Select(e => e.Course.TrackId)
                .Distinct()
                .ToListAsync();

            var vm = new InitiativeDetailViewModel
            {
                Initiative = initiative,
                Tracks = initiative.Tracks.Select(t => new TrackDetailItem
                {
                    Track       = t,
                    CourseCount = t.Courses.Count,
                    IsEnrolled  = enrolledTrackIds.Contains(t.Id)
                }).ToList()
            };

            return View(vm);
        }

        // ── Enrollment ────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollTrack(long trackId, string? returnUrl)
        {
            if (Uid is null) return NeedLogin();

            var courses = await _db.Courses
                .Where(c => c.TrackId == trackId)
                .ToListAsync();

            var alreadyEnrolled = await _db.Enrollments
                .Where(e => e.UserId == Uid && e.Course.TrackId == trackId)
                .Select(e => e.CourseId)
                .ToListAsync();

            var newEnrollments = courses
                .Where(c => !alreadyEnrolled.Contains(c.Id))
                .Select(c => new Enrollment
                {
                    UserId             = Uid!.Value,
                    CourseId           = c.Id,
                    EnrolledAt         = DateTime.UtcNow,
                    Status             = "not-started",
                    ProgressPercentage = 0
                }).ToList();

            if (newEnrollments.Any())
            {
                _db.Enrollments.AddRange(newEnrollments);
                await _db.SaveChangesAsync();
            }

            return Redirect(returnUrl ?? Url.Action("Dashboard")!);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveTrack(long trackId, string? returnUrl)
        {
            if (Uid is null) return NeedLogin();

            var enrollments = await _db.Enrollments
                .Where(e => e.UserId == Uid && e.Course.TrackId == trackId)
                .ToListAsync();

            _db.Enrollments.RemoveRange(enrollments);
            await _db.SaveChangesAsync();

            return Redirect(returnUrl ?? Url.Action("Dashboard")!);
        }

        // ── My Tracks ─────────────────────────────────────────────────────────
        public async Task<IActionResult> MyTracks()
        {
            if (Uid is null) return NeedLogin();

            var enrollments = await _db.Enrollments
                .Where(e => e.UserId == Uid)
                .Include(e => e.Course).ThenInclude(c => c.Track).ThenInclude(t => t.Initiative)
                .ToListAsync();

            var trackCards = enrollments
                .GroupBy(e => e.Course.TrackId)
                .Select(g =>
                {
                    var track = g.First().Course.Track;
                    var pct   = g.Any()
                        ? (int)Math.Round(g.Average(e => (double)e.ProgressPercentage))
                        : 0;
                    return new TrackCard
                    {
                        Track           = track,
                        CoursesCount    = g.Count(),
                        ProgressPercent = pct
                    };
                }).ToList();

            return View(trackCards);
        }

        public async Task<IActionResult> TrackCourses(long trackId)
        {
            if (Uid is null) return NeedLogin();

            var track = await _db.Tracks.FindAsync(trackId);
            if (track is null) return NotFound();

            var courses = await _db.Courses
                .Where(c => c.TrackId == trackId)
                .OrderBy(c => c.Title)
                .ToListAsync();

            var pairs = new List<CourseEnrollmentPair>();
            foreach (var course in courses)
            {
                var en = await _db.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == Uid && e.CourseId == course.Id)
                    ?? new Enrollment
                    {
                        CourseId           = course.Id,
                        Status             = "not-started",
                        ProgressPercentage = 0
                    };
                pairs.Add(new CourseEnrollmentPair { Course = course, Enrollment = en });
            }

            return View(new TrackCoursesViewModel { Track = track, Courses = pairs });
        }

        // ── My Courses ────────────────────────────────────────────────────────
        // FIX: returns List<CourseEnrollmentPair> to match the updated view model
        public async Task<IActionResult> MyCourses()
        {
            if (Uid is null) return NeedLogin();

            var enrollments = await _db.Enrollments
                .Where(e => e.UserId == Uid)
                .Include(e => e.Course).ThenInclude(c => c.Track)
                .ToListAsync();

            var pairs = enrollments.Select(en => new CourseEnrollmentPair
            {
                Course     = en.Course,
                Enrollment = en
            }).ToList();

            return View(pairs);
        }

        // ── Progress update (AJAX) ────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateRequest req)
        {
            if (Uid is null) return Unauthorized();

            var enrollment = await _db.Enrollments
                .FirstOrDefaultAsync(e => e.Id == req.EnrollmentId && e.UserId == Uid);

            if (enrollment is null) return NotFound();

            enrollment.ProgressPercentage = Math.Clamp(req.Progress, 0, 100);
            enrollment.Status = req.Progress >= 100 ? "completed"
                              : req.Progress >  0   ? "in-progress"
                              : "not-started";

            await _db.SaveChangesAsync();
            return Ok(new { status = enrollment.Status, progress = enrollment.ProgressPercentage });
        }

        // ── Analytics ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Analytics()
        {
            if (Uid is null) return NeedLogin();

            var analytics = await _db.StudentAnalytics
                .FirstOrDefaultAsync(a => a.UserId == Uid);

            if (analytics is null)
            {
                var enrollments    = await _db.Enrollments.Where(e => e.UserId == Uid).ToListAsync();
                var completedCount = enrollments.Count(e => e.Status == "completed");
                var rate = enrollments.Any()
                    ? Math.Round((decimal)completedCount / enrollments.Count * 100, 1)
                    : 0m;

                analytics = new StudentAnalytics
                {
                    UserId                = Uid!.Value,
                    TotalCoursesCompleted = completedCount,
                    CompletionRate        = rate,
                    StrengthAreas         = string.Empty,
                    WeaknessAreas         = string.Empty,
                    UpdatedAt             = DateTime.UtcNow
                };
            }

            return View(analytics);
        }

        // ── Opportunities ─────────────────────────────────────────────────────
        public async Task<IActionResult> Opportunities()
        {
            if (Uid is null) return NeedLogin();
            var list = await _db.Opportunities
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(list);
        }
    }
}
