using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarachiEstateHub.Models
{
    public class Inquiry
    {
        public Inquiry()
        {
            CreatedAt = DateTime.Now;
            Status = "New";
            IsRead = false;
            IsActive = true;
        }

        public int InquiryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Phone]
        [StringLength(30)]
        public string Phone { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [Display(Name = "Read")]
        public bool IsRead { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
