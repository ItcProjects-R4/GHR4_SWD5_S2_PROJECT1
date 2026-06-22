using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;
using PathwayPlatform.Models.ViewModels;

namespace PathwayPlatform.Controllers
{
    public class StudentProfileController : Controller
    {
        private readonly AppDbContext        _db;
        private readonly IWebHostEnvironment _env;

        public StudentProfileController(AppDbContext db, IWebHostEnvironment env)
        {
            _db  = db;
            _env = env;
        }

        private long? Uid => (long?)HttpContext.Session.GetInt32("UserId");
        private IActionResult NeedLogin() => RedirectToAction("Login", "Users");

        // ── View Profile ──────────────────────────────────────────────────────
        // GET /StudentProfile  or  GET /StudentProfile/Index/{id?}
        public async Task<IActionResult> Index(long? id)
        {
            if (Uid is null) return NeedLogin();

            long targetId = id ?? Uid.Value;

            var user = await _db.Users.FindAsync(targetId);
            if (user is null) return NotFound();

            var profile = await _db.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == targetId);

            var vm = BuildViewModel(user, profile);
            ViewBag.IsOwner = (targetId == Uid.Value);

            return View(vm);
        }

        // ── Edit Profile ──────────────────────────────────────────────────────
        // GET /StudentProfile/Edit
        public async Task<IActionResult> Edit()
        {
            if (Uid is null) return NeedLogin();

            var user    = await _db.Users.FindAsync(Uid.Value);
            var profile = await _db.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == Uid.Value);

            // FIX: user should not be null here (we have a valid session),
            // but guard anyway to avoid null-dereference
            if (user is null) return NeedLogin();

            return View(BuildViewModel(user, profile));
        }

        // POST /StudentProfile/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentProfileViewModel vm)
        {
            if (Uid is null) return NeedLogin();

            // Remove file-upload properties from validation — they are optional
            ModelState.Remove(nameof(vm.ProfileImageFile));
            ModelState.Remove(nameof(vm.CVFile));

            if (!ModelState.IsValid)
                return View(vm);

            var profile = await _db.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == Uid.Value);

            bool isNew = profile is null;
            profile ??= new StudentProfile
            {
                UserId    = Uid.Value,
                CreatedAt = DateTime.UtcNow
            };

            // Map all posted fields onto the entity
            profile.FullName     = vm.FullName;
            profile.Bio          = vm.Bio;
            profile.PhoneNumber  = vm.PhoneNumber;
            profile.Address      = vm.Address;
            profile.DateOfBirth  = vm.DateOfBirth;
            profile.Gender       = vm.Gender;
            profile.University   = vm.University;
            profile.Faculty      = vm.Faculty;
            profile.AcademicYear = vm.AcademicYear;
            profile.GPA          = vm.GPA;
            profile.Skills       = vm.Skills;
            profile.LinkedInUrl  = vm.LinkedInUrl;
            profile.GithubUrl    = vm.GithubUrl;
            profile.PortfolioUrl = vm.PortfolioUrl;
            profile.UpdatedAt    = DateTime.UtcNow;

            // ── Handle profile image upload ───────────────────────────────────
            if (vm.ProfileImageFile is { Length: > 0 })
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext     = Path.GetExtension(vm.ProfileImageFile.FileName).ToLowerInvariant();

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.ProfileImageFile),
                        "Only JPG, PNG, and WEBP images are allowed.");
                    return View(vm);
                }

                if (vm.ProfileImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(vm.ProfileImageFile),
                        "Image must be smaller than 5 MB.");
                    return View(vm);
                }

                var fileName = $"avatar_{Uid.Value}{ext}";
                // FIX: guard against null WebRootPath
                var wwwroot  = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var folder   = Path.Combine(wwwroot, "uploads", "avatars");
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await vm.ProfileImageFile.CopyToAsync(stream);

                profile.ProfileImageUrl = $"/uploads/avatars/{fileName}";
            }

            // ── Handle CV upload ──────────────────────────────────────────────
            if (vm.CVFile is { Length: > 0 })
            {
                var ext = Path.GetExtension(vm.CVFile.FileName).ToLowerInvariant();

                if (ext != ".pdf")
                {
                    ModelState.AddModelError(nameof(vm.CVFile), "Only PDF files are allowed for CV.");
                    return View(vm);
                }

                if (vm.CVFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(vm.CVFile), "CV must be smaller than 10 MB.");
                    return View(vm);
                }

                var fileName = $"cv_{Uid.Value}.pdf";
                var wwwroot  = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var folder   = Path.Combine(wwwroot, "uploads", "cvs");
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await vm.CVFile.CopyToAsync(stream);

                profile.CVUrl = $"/uploads/cvs/{fileName}";
            }

            if (isNew)
                _db.StudentProfiles.Add(profile);
            else
                _db.StudentProfiles.Update(profile);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static StudentProfileViewModel BuildViewModel(User user, StudentProfile? profile)
        {
            return new StudentProfileViewModel
            {
                Id             = profile?.Id     ?? 0,
                UserId         = user.Id,
                UserName       = user.Name,
                UserEmail      = user.Email,

                FullName       = profile?.FullName,
                Bio            = profile?.Bio,
                PhoneNumber    = profile?.PhoneNumber,
                Address        = profile?.Address,
                DateOfBirth    = profile?.DateOfBirth,
                Gender         = profile?.Gender,

                University     = profile?.University,
                // FIX: null-coalesce legacy Major for Faculty display
                Faculty        = profile?.Faculty  ?? profile?.Major,
                // FIX: null-coalesce legacy Level for AcademicYear display
                AcademicYear   = profile?.AcademicYear ?? profile?.Level,
                GPA            = profile?.GPA,
                Skills         = profile?.Skills,

                // FIX: null-coalesce legacy alias properties
                LinkedInUrl    = profile?.LinkedInUrl  ?? profile?.LinkedInLink,
                GithubUrl      = profile?.GithubUrl    ?? profile?.GithubLink,
                PortfolioUrl   = profile?.PortfolioUrl,

                CVUrl           = profile?.CVUrl,
                ProfileImageUrl = profile?.ProfileImageUrl,
            };
        }
    }
}
