using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostManagement.Models;
using System.Text.RegularExpressions;

namespace PostManagement.Controllers
{
    public class AuthenController : Controller
    {
        public const string SessionEmail = "_Email";
        public const string SessionUserId = "_Id";

        private readonly PostManagementDbContext _context;

        public AuthenController(PostManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserId,Fullname,Address,Email,Password")] AppUsers appUser)
        {

            if (ModelState.IsValid)
            {
                AppUsers? user = await _context.AppUsers.Where(x => x.Email.ToLower()
                    .Equals(appUser.Email.ToLower())).FirstOrDefaultAsync();
                if (user != null)
                {
                    TempData["error"] += "Email existed!";
                    return View(appUser);
                }

                string password = appUser.Password;
                string checkPassword = ValidatePassword(password);
                if (!checkPassword.Equals(""))
                {
                    TempData["error"] += checkPassword;
                    return View(appUser);
                }

                _context.Add(appUser);

                await _context.SaveChangesAsync();
                return Redirect(nameof(Login));
            }

            return View(appUser);
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string? email, string? password)
        {
            if (ModelState.IsValid)
            {
                var data = _context.AppUsers.Where(s => s.Email.Equals(email) && s.Password.Equals(password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    HttpContext.Session.SetString(SessionEmail, data[0].Email);
                    HttpContext.Session.SetInt32(SessionUserId, data[0].UserId);
                    // return Redirect("~/Home/Index");
                    return Redirect("~/Posts");
                }
                else
                {
                    TempData["error"] = "Login failed";
                    return RedirectToAction("Login");
                }
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string ValidatePassword(string password)
        {
            var hasNum = new Regex(@"[0-9]+");
            var hasUpper = new Regex(@"[A-Z]+");
            var hasSpec = new Regex(@"[.,!;'@#$%^&-_+=]+");

            string notification = string.Empty;

            if (password.Length < 8)
            {
                notification += "Password must have at least 8 characters!\n";
            }

            // if (!hasNum.IsMatch(password))
            // {
            //     notification += "Password must have at least 1 numeric!\n";
            // }

            if (!hasUpper.IsMatch(password))
            {
                notification += "Password must have at least 1 uppercase character!\n";
            }

            // if (!hasSpec.IsMatch(password))
            // {
            //     notification += "Password must have at least 1 special character!\n";
            // }

            return notification;
        }
    }
}