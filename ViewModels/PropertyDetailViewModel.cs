using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class PropertyDetailViewModel
    {
        public Property Property { get; set; }
        public User Agent { get; set; }
        public IEnumerable<PropertyImage> Images { get; set; }
        public IEnumerable<Amenity> Amenities { get; set; }
        public IEnumerable<Property> RelatedProperties { get; set; }
        public InquiryViewModel Inquiry { get; set; }
        public bool IsSaved { get; set; }
    }
}
