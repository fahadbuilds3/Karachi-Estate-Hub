using System;
using System.Collections.Generic;
using System.Linq;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.Services
{
    public class SamplePropertySeeder
    {
        public const string SampleMarker = "[AI Sample]";
        public const string SampleAiPrefix = "Sample AI";

        private static readonly string[] TypeNames =
        {
            "House",
            "Flat / Apartment",
            "Portion",
            "Shop",
            "Plot / Land",
            "Office"
        };

        private static readonly string[] LocationNames =
        {
            "DHA Defence",
            "Clifton",
            "Gulshan-e-Iqbal",
            "Gulistan-e-Jauhar",
            "Bahria Town Karachi",
            "Scheme 33",
            "Malir",
            "North Nazimabad",
            "PECHS",
            "Nazimabad"
        };

        private static readonly string[] AmenityNames =
        {
            "Parking",
            "Security",
            "Electricity",
            "Gas",
            "Water Supply",
            "Lift",
            "Boundary Wall",
            "Near Masjid",
            "Near School",
            "Servant Quarter"
        };

        public SampleSeedResult Seed(KarachiEstateDbContext db)
        {
            var existingSampleCount = db.Properties.Count(p => p.Title.StartsWith(SampleMarker));
            if (existingSampleCount > 0)
            {
                return new SampleSeedResult
                {
                    CreatedCount = 0,
                    Skipped = true,
                    Message = "Sample data already exists. No duplicate records were created."
                };
            }

            EnsureReferenceData(db);

            var owner = db.Users.FirstOrDefault(u => u.Role == "Agent" && u.IsActive)
                ?? db.Users.FirstOrDefault(u => u.Role == "Admin" && u.IsActive)
                ?? db.Users.FirstOrDefault();

            if (owner == null)
            {
                return new SampleSeedResult
                {
                    CreatedCount = 0,
                    Skipped = true,
                    Message = "No users exist. Create an admin or agent before seeding sample properties."
                };
            }

            var propertyTypes = db.PropertyTypes.Where(t => t.IsActive).ToList();
            var locations = db.Locations.Where(l => l.IsActive).ToList();
            var amenities = db.Amenities.Where(a => a.IsActive).ToList();
            var imageUrls = db.PropertyImages
                .Where(i => i.IsActive && i.ImageUrl != null && i.ImageUrl != "")
                .Select(i => i.ImageUrl)
                .Distinct()
                .ToList();

            var samples = BuildSamples();
            var createdProperties = new List<Property>();

            for (var i = 0; i < samples.Count; i++)
            {
                var sample = samples[i];
                var property = new Property
                {
                    Title = SampleMarker + " " + sample.Title,
                    Description = sample.Description,
                    Purpose = sample.Purpose,
                    Status = sample.Status,
                    Price = sample.Price,
                    PriceLabel = FormatPriceLabel(sample.Price, sample.Purpose),
                    PriceType = sample.Purpose == "Rent" ? "Monthly" : "Total",
                    Bedrooms = sample.Bedrooms,
                    Bathrooms = sample.Bathrooms,
                    AreaSize = sample.AreaSize,
                    AreaUnit = sample.AreaUnit,
                    Address = sample.Location,
                    StreetAddress = sample.Location + ", Karachi",
                    PhaseOrBlock = sample.Block,
                    ContactNumber = owner.Phone,
                    WhatsAppNumber = owner.Phone,
                    ContactEmail = owner.Email,
                    BadgeText = sample.Purpose == "Rent" ? "For Rent" : "For Sale",
                    GradientCss = sample.GradientCss,
                    IconClass = sample.IconClass,
                    PhotoCount = imageUrls.Any() ? 1 : 0,
                    ViewsCount = (i * 7) % 180,
                    IsActive = sample.Status != "Draft" && sample.Status != "Rejected",
                    IsFeatured = i % 7 == 0 || sample.Title.Contains("Prime"),
                    IsVerified = i % 3 != 0,
                    CreatedAt = DateTime.Now.AddDays(-i),
                    PropertyTypeId = FindType(propertyTypes, sample.Type).PropertyTypeId,
                    LocationId = FindLocation(locations, sample.Location).LocationId,
                    UserId = owner.UserId
                };

                if (imageUrls.Any())
                {
                    property.Images.Add(new PropertyImage
                    {
                        ImageUrl = imageUrls[i % imageUrls.Count],
                        AltText = sample.Title,
                        IsPrimary = true,
                        SortOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                createdProperties.Add(property);
                db.Properties.Add(property);
            }

            db.SaveChanges();

            AddAmenities(db, createdProperties, amenities);
            db.SaveChanges();

            return new SampleSeedResult
            {
                CreatedCount = createdProperties.Count,
                Skipped = false,
                Message = createdProperties.Count + " sample properties were added successfully."
            };
        }

        public SampleSeedResult SeedSampleProperties(KarachiEstateDbContext db)
        {
            var owner = db.Users.FirstOrDefault(u => u.Role == "Agent" && u.IsActive);
            if (owner == null)
            {
                return new SampleSeedResult
                {
                    CreatedCount = 0,
                    Skipped = true,
                    Message = "No active Agent user exists. Create or activate an Agent before seeding sample properties."
                };
            }

            var propertyTypes = db.PropertyTypes.Where(t => t.IsActive).ToList();
            var locations = db.Locations.Where(l => l.IsActive).ToList();

            if (!propertyTypes.Any() || !locations.Any())
            {
                return new SampleSeedResult
                {
                    CreatedCount = 0,
                    Skipped = true,
                    Message = "Property types or locations are missing. Seed reference data before adding sample properties."
                };
            }

            var samples = BuildSamples().Take(40).ToList();
            var createdCount = 0;

            for (var i = 0; i < samples.Count; i++)
            {
                var sample = samples[i];
                var sampleTitle = SampleAiPrefix + " " + (i + 1).ToString("00") + " - " + sample.Title;

                if (db.Properties.Any(p => p.Title == sampleTitle))
                {
                    continue;
                }

                var property = new Property
                {
                    Title = sampleTitle,
                    Description = sample.Description,
                    Purpose = sample.Purpose,
                    Status = "Active",
                    Price = sample.Price,
                    PriceLabel = FormatPriceLabel(sample.Price, sample.Purpose),
                    PriceType = sample.Purpose == "Rent" ? "Monthly" : "Total",
                    Bedrooms = sample.Bedrooms,
                    Bathrooms = sample.Bathrooms,
                    AreaSize = sample.AreaSize,
                    AreaUnit = sample.AreaUnit,
                    Address = sample.Location,
                    StreetAddress = sample.Location + ", Karachi",
                    PhaseOrBlock = sample.Block,
                    ContactNumber = owner.Phone,
                    WhatsAppNumber = owner.Phone,
                    ContactEmail = owner.Email,
                    BadgeText = sample.Purpose == "Rent" ? "For Rent" : "For Sale",
                    GradientCss = sample.GradientCss,
                    IconClass = sample.IconClass,
                    PhotoCount = 0,
                    ViewsCount = (i * 11) % 220,
                    IsActive = true,
                    IsFeatured = i % 6 == 0,
                    IsVerified = i % 3 != 0,
                    CreatedAt = DateTime.Now.AddMinutes(-i),
                    PropertyTypeId = FindType(propertyTypes, sample.Type).PropertyTypeId,
                    LocationId = FindLocation(locations, sample.Location).LocationId,
                    UserId = owner.UserId
                };

                db.Properties.Add(property);
                createdCount++;
            }

            db.SaveChanges();

            if (createdCount == 0)
            {
                return new SampleSeedResult
                {
                    CreatedCount = 0,
                    Skipped = true,
                    Message = "40 Sample AI properties already exist in dbo.Properties. No duplicate records were created."
                };
            }

            return new SampleSeedResult
            {
                CreatedCount = createdCount,
                Skipped = false,
                Message = createdCount + " sample properties added."
            };
        }

        private static void EnsureReferenceData(KarachiEstateDbContext db)
        {
            foreach (var typeName in TypeNames)
            {
                if (!db.PropertyTypes.Any(t => t.Name == typeName))
                {
                    db.PropertyTypes.Add(new PropertyType
                    {
                        Name = typeName,
                        Description = "Sample reference type",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            foreach (var locationName in LocationNames)
            {
                if (!db.Locations.Any(l => l.Name == locationName))
                {
                    db.Locations.Add(new Location
                    {
                        Name = locationName,
                        Area = locationName,
                        City = "Karachi",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            foreach (var amenityName in AmenityNames)
            {
                if (!db.Amenities.Any(a => a.Name == amenityName))
                {
                    db.Amenities.Add(new Amenity
                    {
                        Name = amenityName,
                        IconClass = "ti ti-circle-check",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            db.SaveChanges();
        }

        private static void AddAmenities(KarachiEstateDbContext db, IList<Property> properties, IList<Amenity> amenities)
        {
            if (!amenities.Any())
            {
                return;
            }

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var count = 3 + (i % 4);
                var selectedAmenities = amenities
                    .OrderBy(a => (a.AmenityId + i) % amenities.Count)
                    .Take(count)
                    .ToList();

                foreach (var amenity in selectedAmenities)
                {
                    db.PropertyAmenities.Add(new PropertyAmenity
                    {
                        PropertyId = property.PropertyId,
                        AmenityId = amenity.AmenityId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }

        private static PropertyType FindType(IEnumerable<PropertyType> types, string name)
        {
            return types.FirstOrDefault(t => t.Name == name)
                ?? types.FirstOrDefault(t => t.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                ?? types.First();
        }

        private static Location FindLocation(IEnumerable<Location> locations, string name)
        {
            return locations.FirstOrDefault(l => l.Name == name)
                ?? locations.FirstOrDefault(l => l.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                ?? locations.First();
        }

        private static string FormatPriceLabel(decimal price, string purpose)
        {
            if (purpose == "Rent")
            {
                return "PKR " + price.ToString("N0") + "/month";
            }

            if (price >= 10000000m)
            {
                return "PKR " + (price / 10000000m).ToString("0.##") + " Crore";
            }

            return "PKR " + (price / 100000m).ToString("0.##") + " Lakh";
        }

        private static IList<SamplePropertyRecord> BuildSamples()
        {
            return new List<SamplePropertyRecord>
            {
                S("3 Bedroom Flat in Gulshan under 1 Crore", "Flat / Apartment", "Gulshan-e-Iqbal", "Sale", 8800000m, 3, 3, "1250", "sq ft", "Block 13D", "Active"),
                S("Modern House for Rent in DHA Phase 6", "House", "DHA Defence", "Rent", 275000m, 5, 5, "500", "sq yd", "Phase 6", "Active"),
                S("Ground Floor Shop in North Nazimabad", "Shop", "North Nazimabad", "Sale", 14500000m, 0, 1, "420", "sq ft", "Block H", "Active"),
                S("Residential Plot for Sale in Bahria Town", "Plot / Land", "Bahria Town Karachi", "Sale", 9800000m, 0, 0, "250", "sq yd", "Precinct 10A", "Active"),
                S("Executive Office Space in Clifton", "Office", "Clifton", "Rent", 180000m, 0, 2, "1100", "sq ft", "Block 5", "Active"),
                S("Portion in PECHS under 80 Thousand Rent", "Portion", "PECHS", "Rent", 78000m, 3, 3, "1600", "sq ft", "Block 2", "Active"),
                S("Prime 4 Bedroom Flat near Clifton Sea View", "Flat / Apartment", "Clifton", "Sale", 18500000m, 4, 4, "2200", "sq ft", "Block 2", "Active"),
                S("Independent House in Gulistan-e-Jauhar", "House", "Gulistan-e-Jauhar", "Sale", 42000000m, 6, 6, "400", "sq yd", "Block 15", "Active"),
                S("Small Office for Rent in PECHS", "Office", "PECHS", "Rent", 95000m, 0, 1, "650", "sq ft", "Block 6", "Active"),
                S("Corner Shop for Rent in Malir", "Shop", "Malir", "Rent", 65000m, 0, 1, "300", "sq ft", "Cantt Bazaar", "Active"),
                S("Luxury Villa in DHA Defence", "House", "DHA Defence", "Sale", 135000000m, 6, 7, "1000", "sq yd", "Phase 8", "Active"),
                S("Affordable 2 Bedroom Flat in Nazimabad", "Flat / Apartment", "Nazimabad", "Sale", 7200000m, 2, 2, "900", "sq ft", "Block 3", "Active"),
                S("Upper Portion for Rent in Scheme 33", "Portion", "Scheme 33", "Rent", 70000m, 3, 3, "1800", "sq ft", "Sector 35A", "Active"),
                S("Commercial Plot in Bahria Town Karachi", "Plot / Land", "Bahria Town Karachi", "Sale", 26500000m, 0, 0, "500", "sq yd", "Midway Commercial", "Active"),
                S("Verified Office Suite in DHA", "Office", "DHA Defence", "Rent", 220000m, 0, 2, "1400", "sq ft", "Phase 5", "Active"),
                S("Family House for Rent in Gulshan", "House", "Gulshan-e-Iqbal", "Rent", 155000m, 4, 4, "240", "sq yd", "Block 7", "Active"),
                S("Studio Apartment in Clifton", "Flat / Apartment", "Clifton", "Rent", 85000m, 1, 1, "600", "sq ft", "Block 7", "Active"),
                S("Main Road Shop in PECHS", "Shop", "PECHS", "Sale", 32000000m, 0, 1, "520", "sq ft", "Tariq Road", "Active"),
                S("West Open Plot in Scheme 33", "Plot / Land", "Scheme 33", "Sale", 11800000m, 0, 0, "240", "sq yd", "Sector 17A", "Active"),
                S("Portion for Rent in North Nazimabad", "Portion", "North Nazimabad", "Rent", 90000m, 3, 3, "1700", "sq ft", "Block L", "Active"),
                S("Elegant 3 Bedroom Apartment in DHA", "Flat / Apartment", "DHA Defence", "Sale", 24000000m, 3, 4, "1900", "sq ft", "Phase 7", "Active"),
                S("House for Sale in Malir Cantt", "House", "Malir", "Sale", 38500000m, 5, 5, "350", "sq yd", "Cantt", "Active"),
                S("Office Floor for Sale in Clifton", "Office", "Clifton", "Sale", 55000000m, 0, 4, "3000", "sq ft", "Block 4", "Active"),
                S("Retail Shop in Gulshan-e-Iqbal", "Shop", "Gulshan-e-Iqbal", "Rent", 120000m, 0, 1, "450", "sq ft", "Block 10A", "Active"),
                S("Plot File in Bahria Town Precinct 27", "Plot / Land", "Bahria Town Karachi", "Sale", 6500000m, 0, 0, "125", "sq yd", "Precinct 27", "Active"),
                S("Lower Portion in Nazimabad", "Portion", "Nazimabad", "Rent", 68000m, 2, 2, "1200", "sq ft", "Block 4", "Active"),
                S("Penthouse Flat in Clifton", "Flat / Apartment", "Clifton", "Sale", 68000000m, 4, 5, "3400", "sq ft", "Block 8", "Active"),
                S("Compact Office in Gulistan-e-Jauhar", "Office", "Gulistan-e-Jauhar", "Rent", 72000m, 0, 1, "550", "sq ft", "Block 12", "Active"),
                S("Shop for Rent in Nazimabad Market", "Shop", "Nazimabad", "Rent", 58000m, 0, 1, "260", "sq ft", "Main Market", "Active"),
                S("Brand New House in Scheme 33", "House", "Scheme 33", "Sale", 29500000m, 4, 4, "200", "sq yd", "Saadi Town", "Active"),
                S("2 Bedroom Flat in Gulistan-e-Jauhar", "Flat / Apartment", "Gulistan-e-Jauhar", "Rent", 62000m, 2, 2, "950", "sq ft", "Block 17", "Active"),
                S("Corner Portion for Rent in DHA", "Portion", "DHA Defence", "Rent", 135000m, 3, 3, "2000", "sq ft", "Phase 4", "Active"),
                S("Commercial Office in North Nazimabad", "Office", "North Nazimabad", "Sale", 24500000m, 0, 2, "1300", "sq ft", "Block F", "Active"),
                S("Shop on Main Boulevard Bahria Town", "Shop", "Bahria Town Karachi", "Sale", 22500000m, 0, 1, "390", "sq ft", "Precinct 1", "Active"),
                S("Residential Plot in Malir", "Plot / Land", "Malir", "Sale", 8500000m, 0, 0, "120", "sq yd", "Model Colony", "Active"),
                S("House for Rent in PECHS", "House", "PECHS", "Rent", 190000m, 5, 5, "300", "sq yd", "Block 3", "Active"),
                S("Apartment near University Road", "Flat / Apartment", "Gulshan-e-Iqbal", "Rent", 74000m, 3, 2, "1150", "sq ft", "Block 6", "Active"),
                S("Office for Rent in Shahrah-e-Faisal PECHS", "Office", "PECHS", "Rent", 165000m, 0, 2, "1000", "sq ft", "Block 6", "Active"),
                S("Plot for Sale in DHA City Corridor", "Plot / Land", "DHA Defence", "Sale", 18000000m, 0, 0, "300", "sq yd", "Phase 9", "Active"),
                S("Shop in Clifton Commercial Area", "Shop", "Clifton", "Rent", 210000m, 0, 1, "500", "sq ft", "Block 9", "Active"),
                S("Pending House Submission in Gulshan", "House", "Gulshan-e-Iqbal", "Sale", 36000000m, 5, 5, "240", "sq yd", "Block 3", "Pending"),
                S("Pending Flat in Bahria Town Tower", "Flat / Apartment", "Bahria Town Karachi", "Rent", 52000m, 2, 2, "850", "sq ft", "Precinct 19", "Pending"),
                S("Pending Office in Clifton", "Office", "Clifton", "Sale", 42000000m, 0, 3, "2100", "sq ft", "Block 6", "Pending"),
                S("Draft Shop in DHA Commercial", "Shop", "DHA Defence", "Rent", 140000m, 0, 1, "360", "sq ft", "Phase 2", "Draft"),
                S("Draft Portion in Malir", "Portion", "Malir", "Rent", 55000m, 2, 2, "1000", "sq ft", "Khokhrapar", "Draft"),
                S("Draft Plot in Scheme 33", "Plot / Land", "Scheme 33", "Sale", 7200000m, 0, 0, "120", "sq yd", "Sector 31", "Draft"),
                S("Rejected Office in Nazimabad", "Office", "Nazimabad", "Rent", 88000m, 0, 1, "700", "sq ft", "Block 2", "Rejected"),
                S("Rejected Flat in North Nazimabad", "Flat / Apartment", "North Nazimabad", "Sale", 9500000m, 2, 2, "1000", "sq ft", "Block A", "Rejected"),
                S("Prime Corner House in North Nazimabad", "House", "North Nazimabad", "Sale", 57500000m, 5, 5, "400", "sq yd", "Block B", "Active"),
                S("Sea Facing Office in Clifton", "Office", "Clifton", "Rent", 260000m, 0, 3, "1700", "sq ft", "Block 2", "Active")
            };
        }

        private static SamplePropertyRecord S(string title, string type, string location, string purpose, decimal price, int beds, int baths, string areaSize, string areaUnit, string block, string status)
        {
            return new SamplePropertyRecord
            {
                Title = title,
                Type = type,
                Location = location,
                Purpose = purpose,
                Price = price,
                Bedrooms = beds,
                Bathrooms = baths,
                AreaSize = areaSize,
                AreaUnit = areaUnit,
                Block = block,
                Status = status,
                IconClass = IconFor(type),
                GradientCss = GradientFor(type),
                Description = "Realistic sample listing for testing Karachi Estate Hub AI Assistant searches across Karachi areas, budgets, property types, and purposes."
            };
        }

        private static string IconFor(string type)
        {
            if (type == "Shop") return "ti ti-building-store";
            if (type == "Office") return "ti ti-building-skyscraper";
            if (type == "Plot / Land") return "ti ti-map";
            if (type == "Flat / Apartment") return "ti ti-building-community";
            if (type == "Portion") return "ti ti-home-share";
            return "ti ti-home";
        }

        private static string GradientFor(string type)
        {
            if (type == "Shop") return "linear-gradient(135deg,#D97706,#B45309)";
            if (type == "Office") return "linear-gradient(135deg,#1E3A8A,#0D6B5E)";
            if (type == "Plot / Land") return "linear-gradient(135deg,#475569,#334155)";
            if (type == "Flat / Apartment") return "linear-gradient(135deg,#0D6B5E,#1a8a7a)";
            if (type == "Portion") return "linear-gradient(135deg,#115E59,#0F766E)";
            return "linear-gradient(135deg,#102A43,#0D6B5E)";
        }

        private class SamplePropertyRecord
        {
            public string Title { get; set; }
            public string Type { get; set; }
            public string Location { get; set; }
            public string Purpose { get; set; }
            public decimal Price { get; set; }
            public int Bedrooms { get; set; }
            public int Bathrooms { get; set; }
            public string AreaSize { get; set; }
            public string AreaUnit { get; set; }
            public string Block { get; set; }
            public string Status { get; set; }
            public string IconClass { get; set; }
            public string GradientCss { get; set; }
            public string Description { get; set; }
        }
    }

    public class SampleSeedResult
    {
        public int CreatedCount { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
    }
}
