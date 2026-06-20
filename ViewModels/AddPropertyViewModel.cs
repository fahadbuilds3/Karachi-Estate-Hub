using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class AddPropertyViewModel
    {
        public AddPropertyViewModel()
        {
            SelectedAmenityIds = new List<int>();
            PropertyTypes = new List<SelectListItem>();
            Locations = new List<SelectListItem>();
            Amenities = new List<Amenity>();
            ExistingImages = new List<PropertyImage>();
        }

        public int PropertyId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(10)]
        public string Purpose { get; set; }

        [Required]
        [Display(Name = "Property Type")]
        public int? PropertyTypeId { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Required]
        [Range(1, 9999999999)]
        public decimal? Price { get; set; }

        [StringLength(30)]
        [Display(Name = "Price Type")]
        public string PriceType { get; set; }

        [Range(0, 20)]
        public int Bedrooms { get; set; }

        [Range(0, 20)]
        public int Bathrooms { get; set; }

        [Required]
        [Range(1, 999999)]
        [Display(Name = "Area Size")]
        public decimal? AreaSize { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Area Unit")]
        public string AreaUnit { get; set; }

        [StringLength(100)]
        [Display(Name = "Block / Phase")]
        public string BlockOrPhase { get; set; }

        [StringLength(300)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
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

        public List<int> SelectedAmenityIds { get; set; }
        public IEnumerable<SelectListItem> PropertyTypes { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
        public IEnumerable<Amenity> Amenities { get; set; }
        public IEnumerable<PropertyImage> ExistingImages { get; set; }
    }
}
