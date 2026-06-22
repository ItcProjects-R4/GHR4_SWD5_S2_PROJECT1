using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;
using PathwayPlatform.Models.ViewModels;

namespace PathwayPlatform.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db) => _db = db;

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") is "Admin";
        private IActionResult Guard() => RedirectToAction("Login", "Users");
        private long AdminId => (long)(HttpContext.Session.GetInt32("UserId") ?? 0);

        // ── Overview ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Overview()
        {
            if (!IsAdmin()) return Guard();

            var vm = new AdminOverviewViewModel
            {
                InitiativesCount     = await _db.Initiatives.CountAsync(),
                TracksCount          = await _db.Tracks.CountAsync(),
                CoursesCount         = await _db.Courses.CountAsync(),
                PendingRequestsCount = await _db.CourseRequests.CountAsync(r => r.Status == "pending"),
                OpportunitiesCount   = await _db.Opportunities.CountAsync(),
                UsersCount           = await _db.Users.CountAsync(),
            };

            return View(vm);
        }

        // ── Initiatives ───────────────────────────────────────────────────────
        public async Task<IActionResult> Initiatives()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.Initiatives.Include(i => i.Tracks).OrderBy(i => i.Name).ToListAsync());
        }

        public IActionResult CreateInitiative()
        {
            if (!IsAdmin()) return Guard();
            return View(new Initiative());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInitiative(Initiative model)
        {
            if (!IsAdmin()) return Guard();
            ModelState.Remove("Tracks");
            if (!ModelState.IsValid) return View(model);

            _db.Initiatives.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Initiative created.";
            return RedirectToAction(nameof(Initiatives));
        }

        public async Task<IActionResult> EditInitiative(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Initiatives.Include(i => i.Tracks).FirstOrDefaultAsync(i => i.Id == id);
            return m is null ? NotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInitiative(long id, Initiative model)
        {
            if (!IsAdmin()) return Guard();
            if (id != model.Id) return NotFound();
            ModelState.Remove("Tracks");
            if (!ModelState.IsValid) return View(model);

            _db.Initiatives.Update(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Initiative updated.";
            return RedirectToAction(nameof(Initiatives));
        }

        public async Task<IActionResult> DeleteInitiative(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Initiatives.Include(i => i.Tracks).FirstOrDefaultAsync(i => i.Id == id);
            return m is null ? NotFound() : View(m);
        }

        [HttpPost, ActionName("DeleteInitiative"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInitiativeConfirmed(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Initiatives.FindAsync(id);
            if (m is not null) { _db.Initiatives.Remove(m); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Initiative deleted.";
            return RedirectToAction(nameof(Initiatives));
        }

        // ── Tracks ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Tracks()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.Tracks
                .Include(t => t.Initiative)
                .Include(t => t.Courses)
                .OrderBy(t => t.Name)
                .ToListAsync());
        }

        public async Task<IActionResult> CreateTrack()
        {
            if (!IsAdmin()) return Guard();
            await PopulateInitiativeList();
            return View(new Track());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrack(Track model)
        {
            if (!IsAdmin()) return Guard();
            foreach (var k in new[] { "Initiative", "Courses", "CourseRequests" }) ModelState.Remove(k);
            if (!ModelState.IsValid) { await PopulateInitiativeList(); return View(model); }

            _db.Tracks.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Track created.";
            return RedirectToAction(nameof(Tracks));
        }

        public async Task<IActionResult> EditTrack(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Tracks.FindAsync(id);
            if (m is null) return NotFound();
            await PopulateInitiativeList(m.InitiativeId);
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrack(long id, Track model)
        {
            if (!IsAdmin()) return Guard();
            if (id != model.Id) return NotFound();
            foreach (var k in new[] { "Initiative", "Courses", "CourseRequests" }) ModelState.Remove(k);
            if (!ModelState.IsValid) { await PopulateInitiativeList(model.InitiativeId); return View(model); }

            _db.Tracks.Update(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Track updated.";
            return RedirectToAction(nameof(Tracks));
        }

        public async Task<IActionResult> DeleteTrack(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Tracks.Include(t => t.Initiative).FirstOrDefaultAsync(t => t.Id == id);
            return m is null ? NotFound() : View(m);
        }

        [HttpPost, ActionName("DeleteTrack"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrackConfirmed(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Tracks.FindAsync(id);
            if (m is not null) { _db.Tracks.Remove(m); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Track deleted.";
            return RedirectToAction(nameof(Tracks));
        }

        // ── Courses ───────────────────────────────────────────────────────────
        public async Task<IActionResult> Courses()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.Courses
                .Include(c => c.Track).ThenInclude(t => t.Initiative)
                .OrderBy(c => c.Title)
                .ToListAsync());
        }

        public async Task<IActionResult> CreateCourse()
        {
            if (!IsAdmin()) return Guard();
            await PopulateTrackList();
            return View(new Course());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course model)
        {
            if (!IsAdmin()) return Guard();
            foreach (var k in new[] { "Track", "CourseItems", "Enrollments", "Recommendations", "GeneratedForRequests" })
                ModelState.Remove(k);
            if (!ModelState.IsValid) { await PopulateTrackList(); return View(model); }

            model.CreatedAt = DateTime.UtcNow;
            _db.Courses.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Course created.";
            return RedirectToAction(nameof(Courses));
        }

        public async Task<IActionResult> EditCourse(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Courses.FindAsync(id);
            if (m is null) return NotFound();
            await PopulateTrackList(m.TrackId);
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(long id, Course model)
        {
            if (!IsAdmin()) return Guard();
            if (id != model.Id) return NotFound();
            foreach (var k in new[] { "Track", "CourseItems", "Enrollments", "Recommendations", "GeneratedForRequests" })
                ModelState.Remove(k);
            if (!ModelState.IsValid) { await PopulateTrackList(model.TrackId); return View(model); }

            _db.Courses.Update(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Course updated.";
            return RedirectToAction(nameof(Courses));
        }

        public async Task<IActionResult> DeleteCourse(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Courses.Include(c => c.Track).FirstOrDefaultAsync(c => c.Id == id);
            return m is null ? NotFound() : View(m);
        }

        [HttpPost, ActionName("DeleteCourse"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Courses.FindAsync(id);
            if (m is not null) { _db.Courses.Remove(m); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Course deleted.";
            return RedirectToAction(nameof(Courses));
        }

        // ── Course Requests ───────────────────────────────────────────────────
        public async Task<IActionResult> Requests()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.CourseRequests
                .Include(r => r.User)
                .Include(r => r.Track)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(long id)
        {
            if (!IsAdmin()) return Guard();

            var req = await _db.CourseRequests.FindAsync(id);
            if (req is null) return NotFound();

            req.Status = "approved";

            // Auto-create a Course from the approved request if a track is assigned
            if (req.TrackId.HasValue)
            {
                var course = new Course
                {
                    TrackId     = req.TrackId.Value,
                    Title       = req.Title,
                    Description = req.Description,
                    ContentUrl  = req.ContentUrl,
                    Duration    = req.Duration,
                    SourceType  = "Contributed",
                    CreatedAt   = DateTime.UtcNow
                };
                _db.Courses.Add(course);
                await _db.SaveChangesAsync();
                req.GeneratedCourseId = course.Id;
            }

            _db.CourseValidationLogs.Add(new CourseValidationLog
            {
                CourseRequestId = req.Id,
                ReviewerId      = AdminId,
                Decision        = "approved",
                Comment         = "Approved by admin.",
                ReviewedAt      = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Request approved and course created.";
            return RedirectToAction(nameof(Requests));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(long id, string? comment)
        {
            if (!IsAdmin()) return Guard();

            var req = await _db.CourseRequests.FindAsync(id);
            if (req is null) return NotFound();

            req.Status = "rejected";

            _db.CourseValidationLogs.Add(new CourseValidationLog
            {
                CourseRequestId = req.Id,
                ReviewerId      = AdminId,
                Decision        = "rejected",
                Comment         = comment ?? "Rejected by admin.",
                ReviewedAt      = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            TempData["Info"] = "Request rejected.";
            return RedirectToAction(nameof(Requests));
        }

        // ── Users ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.Enrollments)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync());
        }

        // ── Opportunities ─────────────────────────────────────────────────────
        public async Task<IActionResult> Opportunities()
        {
            if (!IsAdmin()) return Guard();
            return View(await _db.Opportunities.OrderByDescending(o => o.CreatedAt).ToListAsync());
        }

        public IActionResult CreateOpportunity()
        {
            if (!IsAdmin()) return Guard();
            return View(new Opportunity());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOpportunity(Opportunity model)
        {
            if (!IsAdmin()) return Guard();
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.UtcNow;
            _db.Opportunities.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Opportunity created.";
            return RedirectToAction(nameof(Opportunities));
        }

        public async Task<IActionResult> EditOpportunity(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Opportunities.FindAsync(id);
            return m is null ? NotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOpportunity(long id, Opportunity model)
        {
            if (!IsAdmin()) return Guard();
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            _db.Opportunities.Update(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Opportunity updated.";
            return RedirectToAction(nameof(Opportunities));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOpportunity(long id)
        {
            if (!IsAdmin()) return Guard();
            var m = await _db.Opportunities.FindAsync(id);
            if (m is not null) { _db.Opportunities.Remove(m); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Opportunity deleted.";
            return RedirectToAction(nameof(Opportunities));
        }

        // ── Private helpers ───────────────────────────────────────────────────
        private async Task PopulateInitiativeList(long? selectedId = null)
        {
            var list = await _db.Initiatives.OrderBy(i => i.Name).ToListAsync();
            ViewBag.Initiatives = new SelectList(list, "Id", "Name", selectedId);
        }

        private async Task PopulateTrackList(long? selectedId = null)
        {
            var list = await _db.Tracks.Include(t => t.Initiative).OrderBy(t => t.Name).ToListAsync();
            ViewBag.Tracks = new SelectList(list, "Id", "Name", selectedId);
        }
    }
}
