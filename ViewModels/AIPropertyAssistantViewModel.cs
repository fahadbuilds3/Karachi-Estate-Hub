using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.ViewModels
{
    public class AIPropertyAssistantViewModel
    {
        public AIPropertyAssistantViewModel()
        {
            SuggestedProperties = new List<Property>();
            SearchIntent = new PropertySearchIntent();
        }

        [Required(ErrorMessage = "Please type your property request.")]
        [StringLength(500, ErrorMessage = "Please keep your request under 500 characters.")]
        [Display(Name = "Property Request")]
        public string Prompt { get; set; }

        public string AIResponse { get; set; }

        public string ServiceMessage { get; set; }

        public bool UsedGroq { get; set; }

        public PropertySearchIntent SearchIntent { get; set; }

        public IList<Property> SuggestedProperties { get; set; }
    }

    public class PropertySearchIntent
    {
        public string Purpose { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public int? Bedrooms { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
