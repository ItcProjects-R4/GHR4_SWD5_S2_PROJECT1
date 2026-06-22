using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;
using PathwayPlatform.Models.ViewModels;

namespace PathwayPlatform.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db) => _db = db;
        private void StripNavProps()
        {
            foreach (var key in new[]
            {
                "StudentProfile", "StudentAnalytics", "Enrollments",
                "CourseRequests", "Recommendations", "Progresses", "ReviewLogs"
            })
                ModelState.Remove(key);
        }

        // ── Authentication ────────────────────────────────────────────────────

        // GET: /Users/Login
        public IActionResult Login()
        {
            if (AuthHelper.IsLoggedIn(this)) return RedirectToAction("Index", "Home");
            return View(new LoginViewModel());
        }

        // POST: /Users/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user is null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            HttpContext.Session.SetString("UserName",  user.Name);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole",  user.Role.ToString());
            HttpContext.Session.SetInt32 ("UserId",    (int)user.Id);

            return user.Role switch
            {
                UserRole.Admin       => RedirectToAction("Overview",  "Admin"),
                UserRole.Contributor => RedirectToAction("Index",     "Contributor"),
                _                    => RedirectToAction("Dashboard", "Student"),
            };
        }

        // GET: /Users/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // ── Registration ──────────────────────────────────────────────────────

        public IActionResult Register() => View();

        public IActionResult RegisterStudent() => View(new User());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(User user)
        {
            StripNavProps();
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
                ModelState.AddModelError("Email", "Email already registered.");
            if (!ModelState.IsValid) return View(user);

            user.Role      = UserRole.Student;
            user.CreatedAt = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Account created! Please sign in.";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult RegisterContributor() => View(new User());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterContributor(User user)
        {
            StripNavProps();
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
                ModelState.AddModelError("Email", "Email already registered.");
            if (!ModelState.IsValid) return View(user);

            user.Role      = UserRole.Contributor;
            user.CreatedAt = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Contributor account created! Please sign in.";
            return RedirectToAction(nameof(Login));
        }

        // ── Admin: User management ────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            var users = await _db.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.Enrollments)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            if (id is null) return NotFound();

            var user = await _db.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.StudentAnalytics)
                .Include(u => u.Enrollments).ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user is null ? NotFound() : View(user);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            if (id is null) return NotFound();
            var user = await _db.Users.FindAsync(id);
            return user is null ? NotFound() : View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, User user)
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            if (id != user.Id) return NotFound();
            StripNavProps();
            if (!ModelState.IsValid) return View(user);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            TempData["Success"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            if (id is null) return NotFound();
            var user = await _db.Users.FindAsync(id);
            return user is null ? NotFound() : View(user);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (!AuthHelper.IsAdmin(this)) return AuthHelper.RedirectUnauthorized(this);
            var user = await _db.Users.FindAsync(id);
            if (user is not null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "User deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
