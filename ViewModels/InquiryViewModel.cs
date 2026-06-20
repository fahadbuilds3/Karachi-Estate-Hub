using System.ComponentModel.DataAnnotations;

namespace KarachiEstateHub.ViewModels
{
    public class InquiryViewModel
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(30)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
    }
}
