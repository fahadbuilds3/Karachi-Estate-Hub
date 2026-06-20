using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalAgents { get; set; }
        public int TotalNormalUsers { get; set; }
        public int TotalProperties { get; set; }
        public int PendingProperties { get; set; }
        public int ActiveProperties { get; set; }
        public int RejectedProperties { get; set; }
        public int TotalInquiries { get; set; }
        public List<Property> RecentPendingProperties { get; set; }
        public List<User> RecentUsers { get; set; }
    }
}
