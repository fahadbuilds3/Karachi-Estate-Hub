using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KarachiEstateHub.Models;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Controllers
{
    public class PropertiesController : Controller
    {
        private const int MaxImageBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();

        // Public listing page with filters and sorting for active properties.
        public ActionResult Index(
            string purpose,
            int? propertyTypeId,
            int? locationId,
            int? bedrooms,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isFeatured,
            bool? isVerified,
            string sort)
        {
            var properties = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities.Select(pa => pa.Amenity))
                .Where(p => p.Status == "Active" && p.IsActive);

            if (!String.IsNullOrWhiteSpace(purpose))
            {
                properties = properties.Where(p => p.Purpose == purpose);
            }

            if (propertyTypeId.HasValue)
            {
                properties = properties.Where(p => p.PropertyTypeId == propertyTypeId.Value);
            }

            if (locationId.HasValue)
            {
                properties = properties.Where(p => p.LocationId == locationId.Value);
            }

            if (bedrooms.HasValue)
            {
                properties = properties.Where(p => p.Bedrooms >= bedrooms.Value);
            }

            if (minPrice.HasValue)
            {
                properties = properties.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                properties = properties.Where(p => p.Price <= maxPrice.Value);
            }

            if (isFeatured.HasValue)
            {
                properties = properties.Where(p => p.IsFeatured == isFeatured.Value);
            }

            if (isVerified.HasValue)
            {
                properties = properties.Where(p => p.IsVerified == isVerified.Value);
            }

            switch ((sort ?? "newest").ToLowerInvariant())
            {
                case "pricelow":
                    properties = properties.OrderBy(p => p.Price);
                    break;
                case "pricehigh":
                    properties = properties.OrderByDescending(p => p.Price);
                    break;
                default:
                    sort = "newest";
                    properties = properties.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var propertyList = properties.ToList();
            var viewModel = new PropertyListViewModel
            {
                Properties = propertyList,
                PropertyTypes = db.PropertyTypes.Where(pt => pt.IsActive).OrderBy(pt => pt.Name).ToList(),
                Locations = db.Locations.Where(l => l.IsActive).OrderBy(l => l.Name).ToList(),
                Purpose = purpose,
                PropertyTypeId = propertyTypeId,
                LocationId = locationId,
                Bedrooms = bedrooms,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsFeatured = isFeatured,
                IsVerified = isVerified,
                Sort = sort,
                ResultCount = propertyList.Count
            };

            return View(viewModel);
        }

        // Shows property details. Only admins can open non-active listings.
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                TempData["Error"] = "Please select a property to view details.";
                return RedirectToRoute("Listings");
            }

            var userRole = Session["UserRole"] as string;
            var isAdmin = userRole == "Admin";
            var isAgent = userRole == "Agent";
            var canViewNonActiveProperty = isAdmin && !isAgent;

            var query = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities.Select(pa => pa.Amenity))
                .AsQueryable();

            if (!canViewNonActiveProperty)
            {
                query = query.Where(p => p.Status == "Active" && p.IsActive);
            }

            var propertyId = id.Value;
            var property = query.FirstOrDefault(p => p.PropertyId == propertyId);

            if (property == null)
            {
                return HttpNotFound();
            }

            property.ViewsCount += 1;
            property.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            var relatedProperties = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Where(p => p.PropertyId != propertyId
                    && p.Status == "Active"
                    && p.IsActive
                    && (p.LocationId == property.LocationId || p.PropertyTypeId == property.PropertyTypeId))
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .Take(3)
                .ToList();

            var viewModel = new PropertyDetailViewModel
            {
                Property = property,
                Agent = property.User,
                Images = property.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder).ToList(),
                Amenities = property.PropertyAmenities.Select(pa => pa.Amenity).Where(a => a != null).ToList(),
                RelatedProperties = relatedProperties,
                Inquiry = new InquiryViewModel { PropertyId = property.PropertyId },
                IsSaved = IsPropertySaved(property.PropertyId)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendInquiry(InquiryViewModel model)
        {
            var property = db.Properties.FirstOrDefault(p => p.PropertyId == model.PropertyId && p.Status == "Active" && p.IsActive);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please complete all inquiry fields correctly.";
                return RedirectToAction("Details", new { id = model.PropertyId });
            }

            var inquiry = new Inquiry
            {
                PropertyId = model.PropertyId,
                FullName = model.Name,
                Email = model.Email,
                Phone = model.PhoneNumber,
                Message = model.Message,
                UserId = GetCurrentUserId(),
                Status = "Unread",
                IsRead = false,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            db.Inquiries.Add(inquiry);
            db.SaveChanges();

            TempData["Success"] = "Inquiry sent successfully.";
            return RedirectToAction("Details", new { id = model.PropertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(int id)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Login required to save properties.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.RouteUrl("ListingDetails", new { id }) });
            }

            var property = db.Properties.FirstOrDefault(p => p.PropertyId == id && p.Status == "Active" && p.IsActive);
            if (property == null)
            {
                return HttpNotFound();
            }

            var userId = GetCurrentUserId().Value;
            var existing = db.SavedProperties.FirstOrDefault(sp => sp.UserId == userId && sp.PropertyId == id);
            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    existing.SavedAt = DateTime.Now;
                    existing.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                    TempData["Success"] = "Property saved.";
                }
                else
                {
                    TempData["Error"] = "Property is already saved.";
                }

                return RedirectToAction("Details", new { id });
            }

            db.SavedProperties.Add(new SavedProperty
            {
                UserId = userId,
                PropertyId = id,
                IsActive = true,
                SavedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            });
            db.SaveChanges();

            TempData["Success"] = "Property saved.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unsave(int id)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Login required.";
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId().Value;
            var saved = db.SavedProperties.FirstOrDefault(sp => sp.UserId == userId && sp.PropertyId == id && sp.IsActive);
            if (saved != null)
            {
                saved.IsActive = false;
                saved.UpdatedAt = DateTime.Now;
                db.SaveChanges();
                TempData["Success"] = "Property removed from saved.";
            }

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("MySaved"));
        }

        [HttpGet]
        public ActionResult MySaved()
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Login required to view saved properties.";
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId().Value;
            var savedProperties = db.SavedProperties
                .Include(sp => sp.Property)
                .Include(sp => sp.Property.User)
                .Include(sp => sp.Property.PropertyType)
                .Include(sp => sp.Property.Location)
                .Include(sp => sp.Property.Images)
                .Where(sp => sp.UserId == userId && sp.IsActive && sp.Property.Status == "Active" && sp.Property.IsActive)
                .OrderByDescending(sp => sp.SavedAt)
                .ToList();

            return View(savedProperties);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var model = new AddPropertyViewModel
            {
                Purpose = "Sale",
                PriceType = "Total",
                AreaUnit = "sq ft",
                ContactNumber = GetCurrentUserPhone(),
                WhatsAppNumber = GetCurrentUserPhone(),
                ContactEmail = GetCurrentUserEmail()
            };

            PopulatePropertyFormData(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddPropertyViewModel model, IEnumerable<HttpPostedFileBase> images, string submitAction)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            ValidateImages(images);
            if (!ModelState.IsValid)
            {
                PopulatePropertyFormData(model);
                return View(model);
            }

            var property = new Property();
            ApplyPropertyFields(property, model);
            property.UserId = GetCurrentUserId().Value;
            property.Status = submitAction == "Draft" ? "Draft" : "Pending";
            property.CreatedAt = DateTime.Now;
            property.IsActive = true;
            property.IsFeatured = false;
            property.IsVerified = false;

            db.Properties.Add(property);
            db.SaveChanges();

            UpdateAmenities(property.PropertyId, model.SelectedAmenityIds);
            SaveUploadedImages(property.PropertyId, images, false);
            db.SaveChanges();

            TempData["Success"] = property.Status == "Draft"
                ? "Property saved as draft."
                : "Property submitted successfully and is pending review.";

            return RedirectToAction("MyProperties");
        }

        [HttpGet]
        public ActionResult MyProperties()
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var userId = GetCurrentUserId().Value;
            var role = GetCurrentUserRole();
            var properties = db.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.User)
                .Where(p => role == "Admin" || p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(properties);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var property = LoadManageableProperty(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (!CanManageProperty(property))
            {
                TempData["Error"] = "Access denied. You can edit only your own properties.";
                return RedirectToAction("MyProperties");
            }

            var model = ToAddPropertyViewModel(property);
            PopulatePropertyFormData(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AddPropertyViewModel model, IEnumerable<HttpPostedFileBase> images)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var property = LoadManageableProperty(model.PropertyId);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (!CanManageProperty(property))
            {
                TempData["Error"] = "Access denied. You can edit only your own properties.";
                return RedirectToAction("MyProperties");
            }

            ValidateImages(images);
            if (!ModelState.IsValid)
            {
                model.ExistingImages = property.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder).ToList();
                PopulatePropertyFormData(model);
                return View(model);
            }

            var wasActive = property.Status == "Active";
            ApplyPropertyFields(property, model);
            property.UpdatedAt = DateTime.Now;

            if (wasActive && GetCurrentUserRole() == "Agent")
            {
                property.Status = "Pending";
                property.IsVerified = false;
            }

            UpdateAmenities(property.PropertyId, model.SelectedAmenityIds);
            SaveUploadedImages(property.PropertyId, images, property.Images.Any());
            db.SaveChanges();

            TempData["Success"] = "Property updated successfully.";
            return RedirectToAction("MyProperties");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var property = LoadManageableProperty(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (!CanManageProperty(property))
            {
                TempData["Error"] = "Access denied. You can delete only your own properties.";
                return RedirectToAction("MyProperties");
            }

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var access = RequireAgentOrAdmin();
            if (access != null)
            {
                return access;
            }

            var property = LoadManageableProperty(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (!CanManageProperty(property))
            {
                TempData["Error"] = "Access denied. You can delete only your own properties.";
                return RedirectToAction("MyProperties");
            }

            foreach (var image in property.Images.ToList())
            {
                DeleteImageFile(image.ImageUrl);
                db.PropertyImages.Remove(image);
            }

            foreach (var propertyAmenity in property.PropertyAmenities.ToList())
            {
                db.PropertyAmenities.Remove(propertyAmenity);
            }

            foreach (var inquiry in property.Inquiries.ToList())
            {
                db.Inquiries.Remove(inquiry);
            }

            foreach (var savedProperty in property.SavedProperties.ToList())
            {
                db.SavedProperties.Remove(savedProperty);
            }

            db.Properties.Remove(property);
            db.SaveChanges();

            TempData["Success"] = "Property deleted successfully.";
            return RedirectToAction("MyProperties");
        }

        private ActionResult RequireAgentOrAdmin()
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Please login to continue.";
                return RedirectToAction("Login", "Account");
            }

            var role = GetCurrentUserRole();
            if (role != "Agent" && role != "Admin")
            {
                TempData["Error"] = "Access denied. Only agents and admins can manage properties.";
                return RedirectToAction("AccessDenied", "Home");
            }

            return null;
        }

        private int? GetCurrentUserId()
        {
            if (Session["UserId"] == null)
            {
                return null;
            }

            return Convert.ToInt32(Session["UserId"]);
        }

        private bool IsPropertySaved(int propertyId)
        {
            var userId = GetCurrentUserId();
            return userId.HasValue && db.SavedProperties.Any(sp => sp.UserId == userId.Value && sp.PropertyId == propertyId && sp.IsActive);
        }

        private string GetCurrentUserRole()
        {
            return Session["UserRole"] as string;
        }

        private string GetCurrentUserPhone()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return null;
            }

            var user = db.Users.Find(userId.Value);
            return user != null ? user.Phone : null;
        }

        private string GetCurrentUserEmail()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return null;
            }

            var user = db.Users.Find(userId.Value);
            return user != null ? user.Email : null;
        }

        private bool CanManageProperty(Property property)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            return role == "Admin" || (userId.HasValue && property.UserId == userId.Value);
        }

        private Property LoadManageableProperty(int id)
        {
            return db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.PropertyAmenities.Select(pa => pa.Amenity))
                .Include(p => p.Inquiries)
                .Include(p => p.SavedProperties)
                .FirstOrDefault(p => p.PropertyId == id);
        }

        private void PopulatePropertyFormData(AddPropertyViewModel model)
        {
            model.PropertyTypes = db.PropertyTypes
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Name)
                .Select(pt => new SelectListItem
                {
                    Value = pt.PropertyTypeId.ToString(),
                    Text = pt.Name,
                    Selected = model.PropertyTypeId == pt.PropertyTypeId
                })
                .ToList();

            model.Locations = db.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem
                {
                    Value = l.LocationId.ToString(),
                    Text = l.Name,
                    Selected = model.LocationId == l.LocationId
                })
                .ToList();

            model.Amenities = db.Amenities.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
        }

        private AddPropertyViewModel ToAddPropertyViewModel(Property property)
        {
            decimal areaSize;
            Decimal.TryParse(property.AreaSize, out areaSize);

            return new AddPropertyViewModel
            {
                PropertyId = property.PropertyId,
                Title = property.Title,
                Purpose = property.Purpose,
                PropertyTypeId = property.PropertyTypeId,
                LocationId = property.LocationId,
                Price = property.Price,
                PriceType = property.PriceType,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                AreaSize = areaSize,
                AreaUnit = property.AreaUnit,
                BlockOrPhase = property.PhaseOrBlock,
                StreetAddress = property.StreetAddress,
                Description = property.Description,
                ContactNumber = property.ContactNumber,
                WhatsAppNumber = property.WhatsAppNumber,
                ContactEmail = property.ContactEmail,
                SelectedAmenityIds = property.PropertyAmenities.Select(pa => pa.AmenityId).ToList(),
                ExistingImages = property.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder).ToList()
            };
        }

        private void ApplyPropertyFields(Property property, AddPropertyViewModel model)
        {
            property.Title = model.Title;
            property.Purpose = model.Purpose;
            property.PropertyTypeId = model.PropertyTypeId.Value;
            property.LocationId = model.LocationId.Value;
            property.Price = model.Price.Value;
            property.PriceType = model.PriceType;
            property.PriceLabel = BuildPriceLabel(model.Price.Value, model.Purpose, model.PriceType);
            property.Bedrooms = model.Bedrooms;
            property.Bathrooms = model.Bathrooms;
            property.AreaSize = model.AreaSize.Value.ToString("0.##");
            property.AreaUnit = model.AreaUnit;
            property.PhaseOrBlock = model.BlockOrPhase;
            property.StreetAddress = model.StreetAddress;
            property.Address = BuildAddress(model);
            property.Description = model.Description;
            property.ContactNumber = model.ContactNumber;
            property.WhatsAppNumber = model.WhatsAppNumber;
            property.ContactEmail = model.ContactEmail;
            property.IconClass = String.IsNullOrWhiteSpace(property.IconClass) ? "ti ti-building-estate" : property.IconClass;
            property.GradientCss = String.IsNullOrWhiteSpace(property.GradientCss) ? "linear-gradient(135deg,#0D6B5E,#1a8a7a)" : property.GradientCss;
        }

        private string BuildPriceLabel(decimal price, string purpose, string priceType)
        {
            var label = "PKR " + price.ToString("N0");
            if (purpose == "Rent" || priceType == "Monthly")
            {
                label += "/month";
            }

            return label;
        }

        private string BuildAddress(AddPropertyViewModel model)
        {
            var location = model.LocationId.HasValue ? db.Locations.Find(model.LocationId.Value) : null;
            var parts = new List<string>();

            if (!String.IsNullOrWhiteSpace(model.StreetAddress))
            {
                parts.Add(model.StreetAddress);
            }

            if (!String.IsNullOrWhiteSpace(model.BlockOrPhase))
            {
                parts.Add(model.BlockOrPhase);
            }

            if (location != null)
            {
                parts.Add(location.Name);
            }

            return String.Join(", ", parts);
        }

        private void UpdateAmenities(int propertyId, IEnumerable<int> selectedAmenityIds)
        {
            var existing = db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId).ToList();
            foreach (var propertyAmenity in existing)
            {
                db.PropertyAmenities.Remove(propertyAmenity);
            }

            if (selectedAmenityIds == null)
            {
                return;
            }

            foreach (var amenityId in selectedAmenityIds.Distinct())
            {
                db.PropertyAmenities.Add(new PropertyAmenity
                {
                    PropertyId = propertyId,
                    AmenityId = amenityId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
            }
        }

        private void ValidateImages(IEnumerable<HttpPostedFileBase> images)
        {
            if (images == null)
            {
                return;
            }

            var hasImageError = false;
            foreach (var image in images.Where(i => i != null && i.ContentLength > 0))
            {
                var extension = Path.GetExtension(image.FileName);
                if (String.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension.ToLowerInvariant()))
                {
                    ModelState.AddModelError("", "Only .jpg, .jpeg, .png, and .webp images are allowed.");
                    hasImageError = true;
                }

                if (image.ContentLength > MaxImageBytes)
                {
                    ModelState.AddModelError("", "Each image must be 5MB or smaller.");
                    hasImageError = true;
                }
            }

            if (hasImageError)
            {
                TempData["Error"] = "One or more uploaded images are invalid. Use JPG, JPEG, PNG, or WEBP files up to 5MB each.";
            }
        }

        // Saves uploaded files physically and stores web-friendly relative paths in SQL Server.
        private void SaveUploadedImages(int propertyId, IEnumerable<HttpPostedFileBase> images, bool hasExistingImages)
        {
            if (images == null)
            {
                return;
            }

            var uploadDirectory = Server.MapPath("~/Uploads/Properties/");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var sortOrder = db.PropertyImages.Count(pi => pi.PropertyId == propertyId);
            var hasPrimary = hasExistingImages || db.PropertyImages.Any(pi => pi.PropertyId == propertyId && pi.IsPrimary);

            foreach (var image in images.Where(i => i != null && i.ContentLength > 0))
            {
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = "property-" + Guid.NewGuid().ToString("N") + extension;
                var absolutePath = Path.Combine(uploadDirectory, fileName);
                try
                {
                    image.SaveAs(absolutePath);
                }
                catch
                {
                    TempData["Warning"] = "Some images could not be uploaded. The property data was saved.";
                    continue;
                }

                db.PropertyImages.Add(new PropertyImage
                {
                    PropertyId = propertyId,
                    ImageUrl = "/Uploads/Properties/" + fileName,
                    AltText = "Property image",
                    IsPrimary = !hasPrimary,
                    SortOrder = ++sortOrder,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });

                hasPrimary = true;
            }
        }

        private void DeleteImageFile(string relativePath)
        {
            if (String.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var absolutePath = Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }
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
