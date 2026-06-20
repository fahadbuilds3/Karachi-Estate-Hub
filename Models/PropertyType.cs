using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KarachiEstateHub.Models
{
    public class PropertyType
    {
        public PropertyType()
        {
            Properties = new HashSet<Property>();
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public int PropertyTypeId { get; set; }

        [Required]
        [StringLength(80)]
        [Display(Name = "Property Type")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
