using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SecureEmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace SecureEmployeeManagement.Controllers
{
    public class UnsafeExamplesController : Controller
    {
        private readonly AppDbContext _context;

        public UnsafeExamplesController(AppDbContext context)
        {
            _context = context;
        }

      [HttpPost]
public IActionResult UnsafeLogin(string username, string password, IFormFile xmlFile)
{
    if (xmlFile != null && xmlFile.Length > 0)
    {
        var doc = new XmlDocument(); // GÜVENSİZ YAPI
        using (var stream = xmlFile.OpenReadStream())
        {
            doc.Load(stream); // XML Bomb burada patlar 
        }

        var fileUsername = doc.SelectSingleNode("//username")?.InnerText;
        var filePassword = doc.SelectSingleNode("//password")?.InnerText;

        ViewBag.Result = $"XML ile gelen: {fileUsername} - {filePassword}";
        // Giriş kontrolü yapmadık, açık göstermek için
        return View("~/Views/Account/Login.cshtml");
    }

    // Klasik SQL Injection için giriş
    string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
    var user = _context.Users.FromSqlRaw(query).FirstOrDefault();

    if (user != null)
    {
        HttpContext.Session.SetString("username", user.Username);
        HttpContext.Session.SetString("role", user.Role);
        return RedirectToAction("Index", "Employee");
    }

    ViewBag.Error = "Hatalı giriş!";
    return View("~/Views/Account/Login.cshtml");
}


        public IActionResult BrokenSecret()
        {
            return Content("Bu sayfa sadece giriş yapmış kişilerce görülmeli ama oturum kontrolü yok!");
        }

        [HttpGet]
        public IActionResult XmlLogin()
        {
            return View("~/Views/UnsafeExamples/XmlLogin.cshtml");
        }

        [HttpPost]
        public IActionResult XmlLogin(IFormFile xmlFile)
        {
            if (xmlFile != null && xmlFile.Length > 0)
            {
                var doc = new XmlDocument(); //  Güvensiz XML işlemesi
                using (var stream = xmlFile.OpenReadStream())
                {
                    doc.Load(stream); // XML Bomb burada tetiklenebilir
                }

                var username = doc.SelectSingleNode("//username")?.InnerText;
                var password = doc.SelectSingleNode("//password")?.InnerText;

                ViewBag.Result = $"Giriş bilgisi: {username} - {password}";
            }

            return View("~/Views/UnsafeExamples/XmlLogin.cshtml");
        }
    }
}
