// =============================================================================
//  Program.cs — Pathway Platform
// =============================================================================

using Microsoft.EntityFrameworkCore;
using PathwayPlatform.Models;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Entity Framework Core — SQL Server ───────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Session ───────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name        = ".Pathway.Session";
});

builder.Services.AddHttpContextAccessor();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Error handling ────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

// FIX: Session must come BEFORE any middleware that reads session data
app.UseSession();

app.UseAuthorization();

// ── SidebarProfile middleware ─────────────────────────────────────────────────
// Injects the current student's StudentProfile into HttpContext.Items
// so _AppLayout can render _ProfileCard for students.
// Runs after session is available, before any controller action.
app.Use(async (context, next) =>
{
    var userId = context.Session.GetInt32("UserId");
    var role   = context.Session.GetString("UserRole");

    if (userId.HasValue && role == "Student")
    {
        // Skip DB lookup for static file requests
        var path = context.Request.Path.Value ?? "";
        bool isStaticRequest = path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("/images",  StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("/css",     StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("/js",      StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("/lib",     StringComparison.OrdinalIgnoreCase);

        if (!isStaticRequest)
        {
            // FIX: scoped DB service resolved correctly from request services
            var db = context.RequestServices.GetRequiredService<AppDbContext>();
            var profile = await db.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == (long)userId.Value);

            context.Items["SidebarProfile"] = profile;
        }
    }

    await next();
});

// ── Default route ─────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

// ── Ensure upload directories exist ──────────────────────────────────────────
// FIX: WebRootPath can be null when wwwroot doesn't exist yet;
//      use ContentRootPath fallback and create the directory.
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
var wwwroot = env.WebRootPath
              ?? Path.Combine(env.ContentRootPath, "wwwroot");
Directory.CreateDirectory(wwwroot); // no-op if already exists
foreach (var sub in new[] { "uploads/avatars", "uploads/cvs" })
{
    Directory.CreateDirectory(Path.Combine(wwwroot, sub));
}

// ── Auto-migrate + seed ───────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db  = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
            log.LogInformation("Applied pending database migrations.");
        }

        DataSeeder.Seed(db);
        log.LogInformation("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        // FIX: log but don't throw — allows the app to start even when
        // the database isn't available yet (e.g. running 'dotnet run'
        // before SQL Server is up in development).
        log.LogWarning(ex, "Database migration/seed failed. " +
            "The application will start but may not function correctly until the database is available.");
    }
}

app.Run();
