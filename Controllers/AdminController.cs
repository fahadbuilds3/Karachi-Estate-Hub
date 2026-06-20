using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KarachiEstateHub.Models;
using KarachiEstateHub.Services;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();

        // Admin dashboard for reviewing platform users, listings, and approvals.
        public ActionResult Index()
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = db.Users.Count(),
                TotalAgents = db.Users.Count(u => u.Role == "Agent"),
                TotalNormalUsers = db.Users.Count(u => u.Role == "User"),
                TotalProperties = db.Properties.Count(),
                PendingProperties = db.Properties.Count(p => p.Status == "Pending"),
                ActiveProperties = db.Properties.Count(p => p.Status == "Active"),
                RejectedProperties = db.Properties.Count(p => p.Status == "Rejected"),
                TotalInquiries = db.Inquiries.Count(),
                RecentPendingProperties = db.Properties
                    .Include(p => p.User)
                    .Include(p => p.PropertyType)
                    .Include(p => p.Location)
                    .Where(p => p.Status == "Pending")
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .ToList(),
                RecentUsers = db.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(8)
                    .ToList()
            };

            return View(viewModel);
        }

        public ActionResult Properties(string status, string purpose, int? propertyTypeId, int? locationId, string searchTerm)
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var query = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            if (!String.IsNullOrWhiteSpace(purpose))
            {
                query = query.Where(p => p.Purpose == purpose);
            }

            if (propertyTypeId.HasValue)
            {
                query = query.Where(p => p.PropertyTypeId == propertyTypeId.Value);
            }

            if (locationId.HasValue)
            {
                query = query.Where(p => p.LocationId == locationId.Value);
            }

            if (!String.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            var properties = query.OrderByDescending(p => p.CreatedAt).ToList();
            var viewModel = new AdminPropertiesViewModel
            {
                Properties = properties,
                PropertyTypes = db.PropertyTypes.Where(pt => pt.IsActive).OrderBy(pt => pt.Name).ToList(),
                Locations = db.Locations.Where(l => l.IsActive).OrderBy(l => l.Name).ToList(),
                SelectedStatus = status,
                SelectedPurpose = purpose,
                SelectedPropertyTypeId = propertyTypeId,
                SelectedLocationId = locationId,
                SearchTerm = searchTerm,
                ResultCount = properties.Count
            };

            return View(viewModel);
        }

        public ActionResult Users(string role, string status, string searchTerm)
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var query = db.Users
                .Include(u => u.Properties)
                .Include(u => u.SavedProperties)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (status == "Active")
            {
                query = query.Where(u => u.IsActive);
            }
            else if (status == "Inactive")
            {
                query = query.Where(u => !u.IsActive);
            }

            if (!String.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.Phone.Contains(searchTerm));
            }

            var users = query.OrderByDescending(u => u.CreatedAt).ToList();
            var viewModel = new AdminUsersViewModel
            {
                Users = users,
                SelectedRole = role,
                SelectedStatus = status,
                SearchTerm = searchTerm,
                ResultCount = users.Count
            };

            return View(viewModel);
        }

        public ActionResult SampleData()
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var sampleProperties = db.Properties.Where(p =>
                p.Title.StartsWith(SamplePropertySeeder.SampleMarker)
                || p.Title.StartsWith(SamplePropertySeeder.SampleAiPrefix));
            var viewModel = new SampleDataStatusViewModel
            {
                DatabaseServer = db.Database.Connection.DataSource,
                DatabaseName = db.Database.Connection.Database,
                TotalSampleProperties = sampleProperties.Count(),
                ActiveVisibleSampleProperties = sampleProperties.Count(p => p.Status == "Active" && p.IsActive),
                PendingSampleProperties = sampleProperties.Count(p => p.Status == "Pending"),
                DraftSampleProperties = sampleProperties.Count(p => p.Status == "Draft"),
                RejectedSampleProperties = sampleProperties.Count(p => p.Status == "Rejected"),
                MissingUserReferences = sampleProperties.Count(p => !db.Users.Any(u => u.UserId == p.UserId)),
                MissingTypeReferences = sampleProperties.Count(p => !db.PropertyTypes.Any(t => t.PropertyTypeId == p.PropertyTypeId)),
                MissingLocationReferences = sampleProperties.Count(p => !db.Locations.Any(l => l.LocationId == p.LocationId))
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SeedSampleProperties()
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var connectionString = db.Database.Connection.ConnectionString;
            var beforeCount = db.Properties.Count();
            var existingTestSamples = db.Properties.Count(p => p.Title.StartsWith("Sample AI Test"));

            if (existingTestSamples > 0)
            {
                var duplicateMessage = "Database connection string used: " + connectionString + "\r\n"
                    + "Number of properties before insert: " + beforeCount + "\r\n"
                    + "Number of properties inserted: 0\r\n"
                    + "Number of properties after insert: " + beforeCount + "\r\n"
                    + "Message: Sample AI Test properties already exist. No duplicates were inserted.";

                return Content(duplicateMessage, "text/plain");
            }

            var agent = db.Users.FirstOrDefault(u => u.Role == "Agent" && u.IsActive)
                ?? db.Users.FirstOrDefault(u => u.Role == "Agent");
            if (agent == null)
            {
                return Content("Cannot seed sample data. No Agent user exists in dbo.Users.", "text/plain");
            }

            var location = db.Locations.FirstOrDefault(l => l.IsActive)
                ?? db.Locations.FirstOrDefault();
            if (location == null)
            {
                return Content("Cannot seed sample data. No Location record exists in dbo.Locations.", "text/plain");
            }

            var propertyType = db.PropertyTypes.FirstOrDefault(t => t.IsActive)
                ?? db.PropertyTypes.FirstOrDefault();
            if (propertyType == null)
            {
                return Content("Cannot seed sample data. No PropertyType record exists in dbo.PropertyTypes.", "text/plain");
            }

            for (var i = 1; i <= 10; i++)
            {
                db.Properties.Add(new Property
                {
                    Title = "Sample AI Test " + i.ToString("00") + " - Karachi Property",
                    Description = "Temporary sample property inserted directly through the admin seed test action for database visibility debugging.",
                    Purpose = i % 3 == 0 ? "Rent" : "Sale",
                    Status = "Active",
                    Price = i % 3 == 0 ? 50000m + (i * 10000m) : 6000000m + (i * 750000m),
                    PriceLabel = i % 3 == 0 ? "PKR " + (50000 + (i * 10000)).ToString("N0") + "/month" : "PKR " + (6000000 + (i * 750000)).ToString("N0"),
                    PriceType = i % 3 == 0 ? "Monthly" : "Total",
                    Bedrooms = i % 3 == 0 ? 2 : 3 + (i % 3),
                    Bathrooms = i % 2 == 0 ? 2 : 3,
                    AreaSize = (900 + (i * 120)).ToString(),
                    AreaUnit = "sq ft",
                    Address = location.Name,
                    StreetAddress = location.Name + ", Karachi",
                    PhaseOrBlock = "Sample Block " + i,
                    ContactNumber = agent.Phone,
                    WhatsAppNumber = agent.Phone,
                    ContactEmail = agent.Email,
                    BadgeText = i % 3 == 0 ? "For Rent" : "For Sale",
                    GradientCss = "linear-gradient(135deg,#0D6B5E,#1a8a7a)",
                    IconClass = "ti ti-building-estate",
                    PhotoCount = 0,
                    ViewsCount = 0,
                    IsActive = true,
                    IsFeatured = i % 4 == 0,
                    IsVerified = i % 2 == 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    PropertyTypeId = propertyType.PropertyTypeId,
                    LocationId = location.LocationId,
                    UserId = agent.UserId
                });
            }

            var inserted = db.SaveChanges();
            var afterCount = db.Properties.Count();

            var message = "Database connection string used: " + connectionString + "\r\n"
                + "Number of properties before insert: " + beforeCount + "\r\n"
                + "Number of properties inserted: " + (afterCount - beforeCount) + "\r\n"
                + "Number of properties after insert: " + afterCount + "\r\n"
                + "EF SaveChanges result: " + inserted;

            return Content(message, "text/plain");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SeedSampleData()
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var seeder = new SamplePropertySeeder();
            var result = seeder.Seed(db);

            if (result.Skipped)
            {
                TempData["Warning"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            return RedirectToAction("SampleData");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveProperty(int id)
        {
            return UpdatePropertyStatus(id, "Active", "Property approved.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectProperty(int id)
        {
            return UpdatePropertyStatus(id, "Rejected", "Property rejected.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeatureProperty(int id)
        {
            return UpdatePropertyFlag(id, p => p.IsFeatured = true, "Property featured.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnfeatureProperty(int id)
        {
            return UpdatePropertyFlag(id, p => p.IsFeatured = false, "Property unfeatured.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyProperty(int id)
        {
            return UpdatePropertyFlag(id, p => p.IsVerified = true, "Property verified.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnverifyProperty(int id)
        {
            return UpdatePropertyFlag(id, p => p.IsVerified = false, "Property unverified.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateUser(int id)
        {
            return UpdateUserActiveState(id, true, "User activated.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateUser(int id)
        {
            var currentUserId = Convert.ToInt32(Session["UserId"]);
            if (id == currentUserId)
            {
                TempData["Error"] = "You cannot deactivate your own admin account.";
                return RedirectToAction("Users");
            }

            return UpdateUserActiveState(id, false, "User deactivated.");
        }

        private ActionResult UpdatePropertyStatus(int id, string status, string message)
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var property = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            property.Status = status;
            property.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = message;
            return RedirectToAction("Properties");
        }

        private ActionResult UpdatePropertyFlag(int id, Action<Property> update, string message)
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var property = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            update(property);
            property.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = message;
            return RedirectToAction("Properties");
        }

        private ActionResult UpdateUserActiveState(int id, bool isActive, string message)
        {
            var access = RequireAdmin();
            if (access != null)
            {
                return access;
            }

            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.IsActive = isActive;
            user.Status = isActive ? "Active" : "Inactive";
            user.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = message;
            return RedirectToAction("Users");
        }

        // Shared guard so only Admin users can access admin actions.
        private ActionResult RequireAdmin()
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Please login as admin to continue.";
                return RedirectToAction("Login", "Account");
            }

            var role = Session["UserRole"] as string;
            if (role != "Admin")
            {
                TempData["Error"] = "Access denied. Admin access is required.";
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
