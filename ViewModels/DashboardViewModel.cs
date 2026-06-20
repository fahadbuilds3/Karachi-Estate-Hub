using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int PendingListings { get; set; }
        public int DraftListings { get; set; }
        public int NewInquiries { get; set; }
        public int TotalSaved { get; set; }
        public List<Property> RecentProperties { get; set; }
        public List<Inquiry> RecentInquiries { get; set; }
    }
}
