using System.Collections.Generic;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class AdminUsersViewModel
    {
        public List<User> Users { get; set; }
        public string SelectedRole { get; set; }
        public string SelectedStatus { get; set; }
        public string SearchTerm { get; set; }
        public int ResultCount { get; set; }
    }
}
