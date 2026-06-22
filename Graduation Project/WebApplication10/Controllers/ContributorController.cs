using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;
using PathwayPlatform.Models.ViewModels;

namespace PathwayPlatform.Controllers
{
    public class ContributorController : Controller
    {
        private readonly AppDbContext _db;

        public ContributorController(AppDbContext db) => _db = db;

        private long? Uid => (long?)HttpContext.Session.GetInt32("UserId");
        private IActionResult NeedLogin() => RedirectToAction("Login", "Users");

        // ── Submissions dashboard ─────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            if (Uid is null) return NeedLogin();

            var requests = await _db.CourseRequests
                .Where(r => r.UserId == Uid)
                .Include(r => r.Track)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new ContributorDashboardViewModel
            {
                Requests      = requests,
                PendingCount  = requests.Count(r => r.Status == "pending"),
                ApprovedCount = requests.Count(r => r.Status == "approved"),
                RejectedCount = requests.Count(r => r.Status == "rejected"),
            };

            return View(vm);
        }

        // ── New course request ────────────────────────────────────────────────
        public async Task<IActionResult> NewRequest()
        {
            if (Uid is null) return NeedLogin();

            ViewBag.Tracks = await _db.Tracks
                .Include(t => t.Initiative)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(new CourseRequest());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRequest(CourseRequest model)
        {
            if (Uid is null) return NeedLogin();

            // Remove navigation properties not posted from the form
            foreach (var key in new[] { "User", "Track", "GeneratedCourse", "ValidationLogs" })
                ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                ViewBag.Tracks = await _db.Tracks
                    .Include(t => t.Initiative)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                return View(model);
            }

            model.UserId       = Uid!.Value;
            model.Status       = "pending";
            model.CreatedAt    = DateTime.UtcNow;
            model.ProviderName = HttpContext.Session.GetString("UserName") ?? string.Empty;

            _db.CourseRequests.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Request submitted for admin review.";
            return RedirectToAction(nameof(Index));
        }

        // ── Opportunities (read-only) ─────────────────────────────────────────
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
