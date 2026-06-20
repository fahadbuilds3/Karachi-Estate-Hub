using System;
using System.Linq;
using System.Web.Mvc;
using KarachiEstateHub.Helpers;
using KarachiEstateHub.Models;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();

        // Handles simple session-based login for the student project.
        [HttpGet]
        public ActionResult Login(string returnUrl, string message)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (message == "addProperty")
            {
                TempData["Error"] = "Please login as an agent or admin to add a property.";
            }

            return View(new LoginViewModel { Role = "User" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null
                || !user.IsActive
                || !String.Equals(user.Role, model.Role, StringComparison.OrdinalIgnoreCase)
                || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email, password, role, or inactive account.");
                return View(model);
            }

            Session["UserId"] = user.UserId;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;

            TempData["Success"] = "Login successful. Welcome back, " + user.FullName + ".";

            if (!String.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.Role == "Admin" || user.Role == "Agent")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Index", "Home");
        }

        // Public registration supports User and Agent accounts only.
        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel { Role = "User" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Role != "User" && model.Role != "Agent")
            {
                ModelState.AddModelError("Role", "Please select Buyer / Renter or Agent / Seller.");
                return View(model);
            }

            if (db.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.PhoneNumber,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                Role = model.Role,
                Status = "Active",
                IsActive = true,
                IsVerified = model.Role == "User",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Registration successful. Please login to continue.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
