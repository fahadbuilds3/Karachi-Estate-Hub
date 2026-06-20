using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class AdminPropertiesViewModel
    {
        public List<Property> Properties { get; set; }
        public List<PropertyType> PropertyTypes { get; set; }
        public List<Location> Locations { get; set; }
        public string SelectedStatus { get; set; }
        public string SelectedPurpose { get; set; }
        public int? SelectedPropertyTypeId { get; set; }
        public int? SelectedLocationId { get; set; }
        public string SearchTerm { get; set; }
        public int ResultCount { get; set; }
    }
}
