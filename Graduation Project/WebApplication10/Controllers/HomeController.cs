using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PathwayPlatform.Models;

namespace PathwayPlatform.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role switch
            {
                "Admin"       => RedirectToAction("Overview",  "Admin"),
                "Contributor" => RedirectToAction("Index",     "Contributor"),
                "Student"     => RedirectToAction("Dashboard", "Student"),
                _             => RedirectToAction("Login",     "Users")
            };
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
