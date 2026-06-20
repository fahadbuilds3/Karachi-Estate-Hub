using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class PropertyListViewModel
    {
        public IEnumerable<Property> Properties { get; set; }
        public IEnumerable<PropertyType> PropertyTypes { get; set; }
        public IEnumerable<Location> Locations { get; set; }

        public string Purpose { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? LocationId { get; set; }
        public int? Bedrooms { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsVerified { get; set; }
        public string Sort { get; set; }
        public int ResultCount { get; set; }
    }
}
