using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarachiEstateHub.Models
{
    public class Property
    {
        public Property()
        {
            Images = new HashSet<PropertyImage>();
            PropertyAmenities = new HashSet<PropertyAmenity>();
            Inquiries = new HashSet<Inquiry>();
            SavedProperties = new HashSet<SavedProperty>();
            CreatedAt = DateTime.Now;
            Status = "Pending";
            IsActive = true;
        }

        public int PropertyId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Purpose { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [Range(1, 9999999999)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [StringLength(30)]
        [Display(Name = "Price Label")]
        public string PriceLabel { get; set; }

        [StringLength(30)]
        [Display(Name = "Price Type")]
        public string PriceType { get; set; }

        [Range(0, 20)]
        public int Bedrooms { get; set; }

        [Range(0, 20)]
        public int Bathrooms { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Area Size")]
        public string AreaSize { get; set; }

        [StringLength(30)]
        [Display(Name = "Area Unit")]
        public string AreaUnit { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(300)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        public string PhaseOrBlock { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "WhatsApp Number")]
        public string WhatsAppNumber { get; set; }

        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [StringLength(50)]
        public string BadgeText { get; set; }

        [StringLength(120)]
        public string GradientCss { get; set; }

        [StringLength(60)]
        public string IconClass { get; set; }

        [Range(0, 100)]
        public int PhotoCount { get; set; }

        [Display(Name = "Views")]
        public int ViewsCount { get; set; }

        public bool IsActive { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [Display(Name = "Property Type")]
        public int PropertyTypeId { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Required]
        [Display(Name = "Agent")]
        public int UserId { get; set; }

        [ForeignKey("PropertyTypeId")]
        public virtual PropertyType PropertyType { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<PropertyImage> Images { get; set; }
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; }
        public virtual ICollection<Inquiry> Inquiries { get; set; }
        public virtual ICollection<SavedProperty> SavedProperties { get; set; }
    }
}
