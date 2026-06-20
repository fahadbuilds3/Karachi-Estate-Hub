using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarachiEstateHub.Models
{
    public class PropertyAmenity
    {
        public PropertyAmenity()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public int PropertyId { get; set; }

        public int AmenityId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }

        [ForeignKey("AmenityId")]
        public virtual Amenity Amenity { get; set; }
    }
}
