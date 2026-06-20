using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KarachiEstateHub.Models
{
    public class Location
    {
        public Location()
        {
            Properties = new HashSet<Property>();
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Location")]
        public string Name { get; set; }

        [StringLength(100)]
        public string Area { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
