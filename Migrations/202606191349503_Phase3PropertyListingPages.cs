namespace KarachiEstateHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Phase3PropertyListingPages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Amenities",
                c => new
                    {
                        AmenityId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        IconClass = c.String(maxLength: 60),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AmenityId);
            
            CreateTable(
                "dbo.PropertyAmenities",
                c => new
                    {
                        PropertyId = c.Int(nullable: false),
                        AmenityId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.PropertyId, t.AmenityId })
                .ForeignKey("dbo.Amenities", t => t.AmenityId)
                .ForeignKey("dbo.Properties", t => t.PropertyId)
                .Index(t => t.PropertyId)
                .Index(t => t.AmenityId);
            
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false, maxLength: 2000),
                        Purpose = c.String(nullable: false, maxLength: 10),
                        Status = c.String(nullable: false, maxLength: 20),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceLabel = c.String(maxLength: 30),
                        Bedrooms = c.Int(nullable: false),
                        Bathrooms = c.Int(nullable: false),
                        AreaSize = c.String(nullable: false, maxLength: 50),
                        AreaUnit = c.String(maxLength: 30),
                        Address = c.String(maxLength: 300),
                        PhaseOrBlock = c.String(maxLength: 100),
                        BadgeText = c.String(maxLength: 50),
                        GradientCss = c.String(maxLength: 120),
                        IconClass = c.String(maxLength: 60),
                        PhotoCount = c.Int(nullable: false),
                        ViewsCount = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        PropertyTypeId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.PropertyTypes", t => t.PropertyTypeId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.PropertyTypeId)
                .Index(t => t.LocationId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PropertyImages",
                c => new
                    {
                        PropertyImageId = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(nullable: false, maxLength: 500),
                        AltText = c.String(maxLength: 150),
                        IsPrimary = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        PropertyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyImageId)
                .ForeignKey("dbo.Properties", t => t.PropertyId)
                .Index(t => t.PropertyId);
            
            CreateTable(
                "dbo.Inquiries",
                c => new
                    {
                        InquiryId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(maxLength: 30),
                        Message = c.String(nullable: false, maxLength: 1000),
                        Status = c.String(maxLength: 20),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        PropertyId = c.Int(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.InquiryId)
                .ForeignKey("dbo.Properties", t => t.PropertyId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.PropertyId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(maxLength: 30),
                        PasswordHash = c.String(nullable: false, maxLength: 200),
                        Role = c.String(nullable: false, maxLength: 20),
                        Status = c.String(maxLength: 20),
                        IsActive = c.Boolean(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.SavedProperties",
                c => new
                    {
                        SavedPropertyId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PropertyId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.SavedPropertyId)
                .ForeignKey("dbo.Properties", t => t.PropertyId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.PropertyId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Area = c.String(maxLength: 100),
                        City = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.LocationId);
            
            CreateTable(
                "dbo.PropertyTypes",
                c => new
                    {
                        PropertyTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Description = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PropertyTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertyAmenities", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.Properties", "UserId", "dbo.Users");
            DropForeignKey("dbo.Properties", "PropertyTypeId", "dbo.PropertyTypes");
            DropForeignKey("dbo.Properties", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Inquiries", "UserId", "dbo.Users");
            DropForeignKey("dbo.SavedProperties", "UserId", "dbo.Users");
            DropForeignKey("dbo.SavedProperties", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.Inquiries", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.PropertyImages", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.PropertyAmenities", "AmenityId", "dbo.Amenities");
            DropIndex("dbo.SavedProperties", new[] { "PropertyId" });
            DropIndex("dbo.SavedProperties", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Inquiries", new[] { "UserId" });
            DropIndex("dbo.Inquiries", new[] { "PropertyId" });
            DropIndex("dbo.PropertyImages", new[] { "PropertyId" });
            DropIndex("dbo.Properties", new[] { "UserId" });
            DropIndex("dbo.Properties", new[] { "LocationId" });
            DropIndex("dbo.Properties", new[] { "PropertyTypeId" });
            DropIndex("dbo.PropertyAmenities", new[] { "AmenityId" });
            DropIndex("dbo.PropertyAmenities", new[] { "PropertyId" });
            DropTable("dbo.PropertyTypes");
            DropTable("dbo.Locations");
            DropTable("dbo.SavedProperties");
            DropTable("dbo.Users");
            DropTable("dbo.Inquiries");
            DropTable("dbo.PropertyImages");
            DropTable("dbo.Properties");
            DropTable("dbo.PropertyAmenities");
            DropTable("dbo.Amenities");
        }
    }
}
