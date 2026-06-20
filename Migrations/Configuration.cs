namespace KarachiEstateHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using KarachiEstateHub.Helpers;
    using KarachiEstateHub.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<KarachiEstateDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "KarachiEstateHub.Models.KarachiEstateDbContext";
        }

        protected override void Seed(KarachiEstateDbContext context)
        {
            var now = DateTime.Now;

            context.Users.AddOrUpdate(
                u => u.Email,
                new User
                {
                    UserId = 1,
                    FullName = "Karachi Estate Admin",
                    Email = "admin@karachiestatehub.pk",
                    Phone = "+92 21 0000000",
                    PasswordHash = PasswordHelper.HashPassword("Admin123"),
                    Role = "Admin",
                    Status = "Active",
                    IsActive = true,
                    IsVerified = true,
                    CreatedAt = now
                },
                new User
                {
                    UserId = 2,
                    FullName = "Ahmed Khan",
                    Email = "agent@karachiestatehub.pk",
                    Phone = "+92 300 0000000",
                    PasswordHash = PasswordHelper.HashPassword("Agent123"),
                    Role = "Agent",
                    Status = "Active",
                    IsActive = true,
                    IsVerified = true,
                    CreatedAt = now
                });

            context.PropertyTypes.AddOrUpdate(
                pt => pt.Name,
                new PropertyType { PropertyTypeId = 1, Name = "House", Description = "Residential houses and villas", IsActive = true, CreatedAt = now },
                new PropertyType { PropertyTypeId = 2, Name = "Flat / Apartment", Description = "Apartments and flats", IsActive = true, CreatedAt = now },
                new PropertyType { PropertyTypeId = 3, Name = "Portion", Description = "Upper and lower portions", IsActive = true, CreatedAt = now },
                new PropertyType { PropertyTypeId = 4, Name = "Shop", Description = "Commercial shops", IsActive = true, CreatedAt = now },
                new PropertyType { PropertyTypeId = 5, Name = "Plot / Land", Description = "Residential and commercial plots", IsActive = true, CreatedAt = now },
                new PropertyType { PropertyTypeId = 6, Name = "Office", Description = "Commercial offices", IsActive = true, CreatedAt = now });

            context.Locations.AddOrUpdate(
                l => l.Name,
                new Location { LocationId = 1, Name = "DHA Defence", Area = "DHA", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 2, Name = "Clifton", Area = "Clifton", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 3, Name = "Gulshan-e-Iqbal", Area = "Gulshan-e-Iqbal", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 4, Name = "Gulistan-e-Jauhar", Area = "Gulistan-e-Jauhar", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 5, Name = "Bahria Town Karachi", Area = "Bahria Town", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 6, Name = "Scheme 33", Area = "Scheme 33", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 7, Name = "Malir", Area = "Malir", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 8, Name = "North Nazimabad", Area = "North Nazimabad", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 9, Name = "PECHS", Area = "PECHS", City = "Karachi", IsActive = true, CreatedAt = now },
                new Location { LocationId = 10, Name = "Nazimabad", Area = "Nazimabad", City = "Karachi", IsActive = true, CreatedAt = now });

            context.Amenities.AddOrUpdate(
                a => a.Name,
                new Amenity { AmenityId = 1, Name = "Bedrooms", IconClass = "ti ti-bed", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 2, Name = "Bathrooms", IconClass = "ti ti-bath", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 3, Name = "Parking", IconClass = "ti ti-car", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 4, Name = "Security", IconClass = "ti ti-shield-check", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 5, Name = "Backup Generator", IconClass = "ti ti-bolt", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 6, Name = "Elevator", IconClass = "ti ti-elevator", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 7, Name = "Servant Quarter", IconClass = "ti ti-home", IsActive = true, CreatedAt = now },
                new Amenity { AmenityId = 8, Name = "Main Road Access", IconClass = "ti ti-road", IsActive = true, CreatedAt = now });

            context.Properties.AddOrUpdate(
                p => p.Title,
                new Property
                {
                    PropertyId = 1,
                    Title = "Modern 5-Bedroom Corner House",
                    Description = "A modern corner house with spacious rooms, premium finishes, and family-friendly planning.",
                    Purpose = "Sale",
                    Status = "Active",
                    Price = 25000000,
                    PriceLabel = "PKR 2.5 Crore",
                    Bedrooms = 5,
                    Bathrooms = 5,
                    AreaSize = "10",
                    AreaUnit = "Marla",
                    Address = "DHA Defence, Phase 6",
                    PhaseOrBlock = "Phase 6",
                    BadgeText = "Featured",
                    GradientCss = "linear-gradient(135deg,#0D6B5E,#1a8a7a)",
                    IconClass = "ti ti-building-estate",
                    PhotoCount = 8,
                    ViewsCount = 128,
                    IsActive = true,
                    IsFeatured = true,
                    IsVerified = false,
                    CreatedAt = now,
                    PropertyTypeId = 1,
                    LocationId = 1,
                    UserId = 2
                },
                new Property
                {
                    PropertyId = 2,
                    Title = "Luxury 3-Bed Flat with Sea View",
                    Description = "Luxury apartment with sea-facing views, refined interiors, and excellent access to Clifton amenities.",
                    Purpose = "Sale",
                    Status = "Active",
                    Price = 18000000,
                    PriceLabel = "PKR 1.8 Crore",
                    Bedrooms = 3,
                    Bathrooms = 3,
                    AreaSize = "2,200",
                    AreaUnit = "sq ft",
                    Address = "Clifton, Block 5",
                    PhaseOrBlock = "Block 5",
                    BadgeText = "Verified",
                    GradientCss = "linear-gradient(135deg,#1A2744,#2d3e6e)",
                    IconClass = "ti ti-building",
                    PhotoCount = 6,
                    ViewsCount = 96,
                    IsActive = true,
                    IsFeatured = false,
                    IsVerified = true,
                    CreatedAt = now,
                    PropertyTypeId = 2,
                    LocationId = 2,
                    UserId = 2
                },
                new Property
                {
                    PropertyId = 3,
                    Title = "Spacious 3-Bed Family Apartment",
                    Description = "Comfortable family apartment near schools, parks, and daily conveniences.",
                    Purpose = "Rent",
                    Status = "Active",
                    Price = 85000,
                    PriceLabel = "PKR 85,000/month",
                    Bedrooms = 3,
                    Bathrooms = 2,
                    AreaSize = "1,800",
                    AreaUnit = "sq ft",
                    Address = "Gulshan-e-Iqbal, Block 13",
                    PhaseOrBlock = "Block 13",
                    BadgeText = "Featured",
                    GradientCss = "linear-gradient(135deg,#2d7a6e,#3a9988)",
                    IconClass = "ti ti-home",
                    PhotoCount = 5,
                    ViewsCount = 84,
                    IsActive = true,
                    IsFeatured = true,
                    IsVerified = false,
                    CreatedAt = now,
                    PropertyTypeId = 2,
                    LocationId = 3,
                    UserId = 2
                },
                new Property
                {
                    PropertyId = 4,
                    Title = "240 Sq Yard Residential Plot",
                    Description = "Well-positioned residential plot in a planned precinct with strong investment potential.",
                    Purpose = "Sale",
                    Status = "Active",
                    Price = 12000000,
                    PriceLabel = "PKR 1.2 Crore",
                    Bedrooms = 0,
                    Bathrooms = 0,
                    AreaSize = "240",
                    AreaUnit = "sq yd",
                    Address = "Bahria Town, Precinct 10",
                    PhaseOrBlock = "Precinct 10",
                    BadgeText = "",
                    GradientCss = "linear-gradient(135deg,#374151,#4b5563)",
                    IconClass = "ti ti-map",
                    PhotoCount = 4,
                    ViewsCount = 51,
                    IsActive = true,
                    IsFeatured = false,
                    IsVerified = false,
                    CreatedAt = now,
                    PropertyTypeId = 5,
                    LocationId = 5,
                    UserId = 2
                },
                new Property
                {
                    PropertyId = 5,
                    Title = "4-Bed Upper Portion in PECHS",
                    Description = "Upper portion with practical layout, separate access, and a convenient PECHS address.",
                    Purpose = "Rent",
                    Status = "Active",
                    Price = 60000,
                    PriceLabel = "PKR 60,000/month",
                    Bedrooms = 4,
                    Bathrooms = 3,
                    AreaSize = "1,600",
                    AreaUnit = "sq ft",
                    Address = "PECHS, Block 2",
                    PhaseOrBlock = "Block 2",
                    BadgeText = "Verified",
                    GradientCss = "linear-gradient(135deg,#5b4fcf,#7c6ee0)",
                    IconClass = "ti ti-home-2",
                    PhotoCount = 7,
                    ViewsCount = 73,
                    IsActive = true,
                    IsFeatured = false,
                    IsVerified = true,
                    CreatedAt = now,
                    PropertyTypeId = 3,
                    LocationId = 9,
                    UserId = 2
                },
                new Property
                {
                    PropertyId = 6,
                    Title = "Commercial Shop - Ground Floor",
                    Description = "Ground-floor commercial shop with main-road visibility and strong walk-in potential.",
                    Purpose = "Sale",
                    Status = "Active",
                    Price = 8500000,
                    PriceLabel = "PKR 85 Lakh",
                    Bedrooms = 0,
                    Bathrooms = 0,
                    AreaSize = "400",
                    AreaUnit = "sq ft",
                    Address = "North Nazimabad, Block H",
                    PhaseOrBlock = "Block H",
                    BadgeText = "",
                    GradientCss = "linear-gradient(135deg,#b45309,#d97706)",
                    IconClass = "ti ti-building-store",
                    PhotoCount = 3,
                    ViewsCount = 42,
                    IsActive = true,
                    IsFeatured = false,
                    IsVerified = false,
                    CreatedAt = now,
                    PropertyTypeId = 4,
                    LocationId = 8,
                    UserId = 2
                });

            context.PropertyAmenities.AddOrUpdate(
                pa => new { pa.PropertyId, pa.AmenityId },
                new PropertyAmenity { PropertyId = 1, AmenityId = 1, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 1, AmenityId = 2, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 1, AmenityId = 3, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 2, AmenityId = 1, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 2, AmenityId = 2, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 2, AmenityId = 6, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 3, AmenityId = 1, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 3, AmenityId = 2, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 4, AmenityId = 8, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 5, AmenityId = 1, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 5, AmenityId = 2, IsActive = true, CreatedAt = now },
                new PropertyAmenity { PropertyId = 6, AmenityId = 8, IsActive = true, CreatedAt = now });

            context.PropertyImages.AddOrUpdate(
                pi => pi.ImageUrl,
                new PropertyImage { PropertyImageId = 1, PropertyId = 1, ImageUrl = "/Content/images/sample-property-1.jpg", AltText = "Modern 5-bedroom corner house", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now },
                new PropertyImage { PropertyImageId = 2, PropertyId = 2, ImageUrl = "/Content/images/sample-property-2.jpg", AltText = "Luxury 3-bed flat with sea view", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now },
                new PropertyImage { PropertyImageId = 3, PropertyId = 3, ImageUrl = "/Content/images/sample-property-3.jpg", AltText = "Spacious family apartment", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now },
                new PropertyImage { PropertyImageId = 4, PropertyId = 4, ImageUrl = "/Content/images/sample-property-4.jpg", AltText = "Residential plot", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now },
                new PropertyImage { PropertyImageId = 5, PropertyId = 5, ImageUrl = "/Content/images/sample-property-5.jpg", AltText = "Upper portion in PECHS", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now },
                new PropertyImage { PropertyImageId = 6, PropertyId = 6, ImageUrl = "/Content/images/sample-property-6.jpg", AltText = "Commercial shop ground floor", IsPrimary = true, SortOrder = 1, IsActive = true, CreatedAt = now });
        }
    }
}
