using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KarachiEstateHub.Models;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Controllers
{
    public class DashboardController : Controller
    {
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();

        // Agent dashboard shows only the logged-in agent's own data.
        public ActionResult Index()
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            if ((Session["UserRole"] as string) == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            var propertyQuery = GetScopedProperties();
            var inquiryQuery = GetScopedInquiries();
            var properties = propertyQuery.ToList();
            var propertyIds = properties.Select(p => p.PropertyId).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalProperties = properties.Count,
                ActiveListings = properties.Count(p => p.Status == "Active"),
                PendingListings = properties.Count(p => p.Status == "Pending"),
                DraftListings = properties.Count(p => p.Status == "Draft"),
                NewInquiries = inquiryQuery.Count(i => !i.IsRead),
                TotalSaved = db.SavedProperties.Count(sp => sp.IsActive && propertyIds.Contains(sp.PropertyId)),
                RecentProperties = propertyQuery
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .ToList(),
                RecentInquiries = inquiryQuery
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(8)
                    .ToList()
            };

            return View(viewModel);
        }

        public ActionResult Inquiries()
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var inquiries = GetScopedInquiries()
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            return View(inquiries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkInquiryRead(int id)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var inquiry = GetScopedInquiries().FirstOrDefault(i => i.InquiryId == id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }

            inquiry.IsRead = true;
            inquiry.Status = "Read";
            inquiry.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = "Inquiry marked as read.";
            return RedirectToAction("Inquiries");
        }

        private IQueryable<Property> GetScopedProperties()
        {
            var role = Session["UserRole"] as string;
            var userId = Convert.ToInt32(Session["UserId"]);

            var query = db.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.User);

            if (role == "Agent")
            {
                query = query.Where(p => p.UserId == userId);
            }

            return query;
        }

        private IQueryable<Inquiry> GetScopedInquiries()
        {
            var role = Session["UserRole"] as string;
            var userId = Convert.ToInt32(Session["UserId"]);

            var query = db.Inquiries
                .Include(i => i.Property)
                .Include(i => i.Property.Location)
                .Include(i => i.Property.User);

            if (role == "Agent")
            {
                query = query.Where(i => i.Property.UserId == userId);
            }

            return query;
        }

        // Restricts dashboard pages to Agent and Admin users.
        private ActionResult RequireAgentOrAdmin()
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Please login to access the dashboard.";
                return RedirectToAction("Login", "Account");
            }

            var role = Session["UserRole"] as string;
            if (role == "User")
            {
                TempData["Error"] = "Access denied. Dashboard is available for agents and admins only.";
                return RedirectToAction("AccessDenied", "Home");
            }

            if (role != "Admin" && role != "Agent")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("AccessDenied", "Home");
            }

            return null;
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
