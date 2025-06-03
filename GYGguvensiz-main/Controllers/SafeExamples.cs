using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SecureEmployeeManagement.Data;
using System.Linq;

namespace SecureEmployeeManagement.Controllers
{
    public class SafeExamplesController : Controller
    {
        private readonly AppDbContext _context;

        public SafeExamplesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SafeLogin(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("role", user.Role);
                return RedirectToAction("Index", "Employee");
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
            return View("Login");
        }

        public IActionResult SafeSecret()
        {
            if (HttpContext.Session.GetString("username") == null)
                return RedirectToAction("Login", "Account");

            return Content("Bu sayfa sadece oturum açmış kullanıcılar içindir.");
        }
    }
}
