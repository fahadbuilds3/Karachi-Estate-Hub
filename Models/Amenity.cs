using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KarachiEstateHub.Models
{
    public class Amenity
    {
        public Amenity()
        {
            PropertyAmenities = new HashSet<PropertyAmenity>();
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public int AmenityId { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(60)]
        [Display(Name = "Icon Class")]
        public string IconClass { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; }
    }
}
