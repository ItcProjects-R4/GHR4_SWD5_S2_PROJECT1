// AuthHelper.cs — Session utility methods shared by all controllers.
using Microsoft.AspNetCore.Mvc;

namespace PathwayPlatform.Controllers
{
    public static class AuthHelper
    {
        public static int?   GetUserId(Controller c)   => c.HttpContext.Session.GetInt32("UserId");
        public static string GetUserRole(Controller c) => c.HttpContext.Session.GetString("UserRole") ?? "";

        public static bool IsLoggedIn(Controller c)    => GetUserId(c) != null;
        public static bool IsAdmin(Controller c)       => GetUserRole(c) == "Admin";
        public static bool IsContributor(Controller c) => GetUserRole(c) == "Contributor";
        public static bool IsStudent(Controller c)     => GetUserRole(c) == "Student";

        public static IActionResult RedirectUnauthorized(Controller c)
            => c.RedirectToAction("Login", "Users");
    }
}
