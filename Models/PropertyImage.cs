using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarachiEstateHub.Models
{
    public class PropertyImage
    {
        public PropertyImage()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public int PropertyImageId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [StringLength(150)]
        [Display(Name = "Alt Text")]
        public string AltText { get; set; }

        public bool IsPrimary { get; set; }

        [Range(0, 100)]
        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }
    }
}
