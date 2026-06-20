using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KarachiEstateHub.Models
{
    public class User
    {
        public User()
        {
            Properties = new HashSet<Property>();
            Inquiries = new HashSet<Inquiry>();
            SavedProperties = new HashSet<SavedProperty>();
            CreatedAt = DateTime.Now;
            Status = "Active";
            IsActive = true;
        }

        public int UserId { get; set; }

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
        [StringLength(200)]
        [Display(Name = "Password Hash")]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Verified")]
        public bool IsVerified { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Inquiry> Inquiries { get; set; }
        public virtual ICollection<SavedProperty> SavedProperties { get; set; }
    }
}
