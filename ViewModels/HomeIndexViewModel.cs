using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class HomeIndexViewModel
    {
        public int ActiveListingsCount { get; set; }
        public int MajorAreasCount { get; set; }
        public int VerifiedAgentsCount { get; set; }
        public IList<LocationSummaryViewModel> PopularLocations { get; set; }
        public IList<Property> FeaturedProperties { get; set; }
    }

    public class LocationSummaryViewModel
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public int ActiveListingsCount { get; set; }
        public string IconClass { get; set; }
    }
}
