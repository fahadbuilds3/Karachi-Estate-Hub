using System.Data.Entity;

namespace KarachiEstateHub.Models
{
    public class KarachiEstateDbContext : DbContext
    {
        public KarachiEstateDbContext()
            : base("name=KarachiEstateDbContext")
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PropertyType> PropertyTypes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<PropertyImage> PropertyImages { get; set; }
        public virtual DbSet<Amenity> Amenities { get; set; }
        public virtual DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public virtual DbSet<Inquiry> Inquiries { get; set; }
        public virtual DbSet<SavedProperty> SavedProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<PropertyAmenity>()
                .HasKey(pa => new { pa.PropertyId, pa.AmenityId });

            modelBuilder.Entity<PropertyAmenity>()
                .HasRequired(pa => pa.Property)
                .WithMany(p => p.PropertyAmenities)
                .HasForeignKey(pa => pa.PropertyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PropertyAmenity>()
                .HasRequired(pa => pa.Amenity)
                .WithMany(a => a.PropertyAmenities)
                .HasForeignKey(pa => pa.AmenityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Property>()
                .HasRequired(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Property>()
                .HasRequired(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Property>()
                .HasRequired(p => p.Location)
                .WithMany(l => l.Properties)
                .HasForeignKey(p => p.LocationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Property>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PropertyImage>()
                .HasRequired(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inquiry>()
                .HasRequired(i => i.Property)
                .WithMany(p => p.Inquiries)
                .HasForeignKey(i => i.PropertyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inquiry>()
                .HasOptional(i => i.User)
                .WithMany(u => u.Inquiries)
                .HasForeignKey(i => i.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SavedProperty>()
                .HasRequired(sp => sp.User)
                .WithMany(u => u.SavedProperties)
                .HasForeignKey(sp => sp.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SavedProperty>()
                .HasRequired(sp => sp.Property)
                .WithMany(p => p.SavedProperties)
                .HasForeignKey(sp => sp.PropertyId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
