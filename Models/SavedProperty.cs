using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarachiEstateHub.Models
{
    public class SavedProperty
    {
        public SavedProperty()
        {
            CreatedAt = DateTime.Now;
            SavedAt = DateTime.Now;
            IsActive = true;
        }

        public int SavedPropertyId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Saved At")]
        public DateTime SavedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }
    }
}
