using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using KarachiEstateHub.Models;
using KarachiEstateHub.Services;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Controllers
{
    public class AIController : Controller
    {
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();
        private readonly GroqPropertyAssistantService groqService = new GroqPropertyAssistantService();

        [HttpGet]
        public ActionResult Assistant()
        {
            return View(new AIPropertyAssistantViewModel
            {
                AIResponse = "Tell me what you are looking for, for example: 3 bedroom flat in Gulshan under 1 crore."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assistant(AIPropertyAssistantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SuggestedProperties = new List<Property>();
                return View(model);
            }

            PropertySearchIntent intent;
            string serviceMessage;
            var usedGroq = groqService.TryExtractIntent(model.Prompt, out intent, out serviceMessage);

            if (!usedGroq)
            {
                intent = ParseLocalIntent(model.Prompt);
            }

            model.SearchIntent = NormalizeIntent(intent);
            model.UsedGroq = usedGroq;
            model.ServiceMessage = serviceMessage;
            model.SuggestedProperties = FindMatchingProperties(model.SearchIntent);
            model.AIResponse = BuildFriendlyResponse(model.SearchIntent, model.SuggestedProperties.Count, usedGroq);

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private PropertySearchIntent ParseLocalIntent(string prompt)
        {
            var text = (prompt ?? String.Empty).ToLowerInvariant();
            var intent = new PropertySearchIntent();

            if (ContainsAny(text, "rent", "rental", "for rent"))
            {
                intent.Purpose = "Rent";
            }
            else if (ContainsAny(text, "buy", "sale", "for sale", "purchase"))
            {
                intent.Purpose = "Sale";
            }

            var propertyTypes = db.PropertyTypes.Where(t => t.IsActive).ToList();
            intent.PropertyType = propertyTypes
                .Select(t => t.Name)
                .FirstOrDefault(name => ContainsPropertyType(text, name));

            if (String.IsNullOrWhiteSpace(intent.PropertyType))
            {
                if (ContainsAny(text, "flat", "apartment"))
                {
                    intent.PropertyType = "Flat";
                }
                else if (text.Contains("house"))
                {
                    intent.PropertyType = "House";
                }
                else if (text.Contains("shop"))
                {
                    intent.PropertyType = "Shop";
                }
                else if (ContainsAny(text, "plot", "land"))
                {
                    intent.PropertyType = "Plot";
                }
                else if (text.Contains("office"))
                {
                    intent.PropertyType = "Office";
                }
            }

            var locations = db.Locations.Where(l => l.IsActive).ToList();
            intent.Location = locations
                .Select(l => l.Name)
                .FirstOrDefault(name => ContainsLocation(text, name));

            intent.Bedrooms = ExtractBedrooms(text);
            ApplyPriceIntent(text, intent);

            return intent;
        }

        private PropertySearchIntent NormalizeIntent(PropertySearchIntent intent)
        {
            intent = intent ?? new PropertySearchIntent();

            if (!String.IsNullOrWhiteSpace(intent.Purpose))
            {
                if (intent.Purpose.Equals("rent", StringComparison.OrdinalIgnoreCase))
                {
                    intent.Purpose = "Rent";
                }
                else if (intent.Purpose.Equals("sale", StringComparison.OrdinalIgnoreCase) || intent.Purpose.Equals("buy", StringComparison.OrdinalIgnoreCase))
                {
                    intent.Purpose = "Sale";
                }
                else
                {
                    intent.Purpose = null;
                }
            }

            return intent;
        }

        private IList<Property> FindMatchingProperties(PropertySearchIntent intent)
        {
            var query = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Where(p => p.Status == "Active" && p.IsActive);

            if (!String.IsNullOrWhiteSpace(intent.Purpose))
            {
                query = query.Where(p => p.Purpose == intent.Purpose);
            }

            if (intent.Bedrooms.HasValue)
            {
                query = query.Where(p => p.Bedrooms >= intent.Bedrooms.Value);
            }

            if (intent.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= intent.MinPrice.Value);
            }

            if (intent.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= intent.MaxPrice.Value);
            }

            var properties = query
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.IsVerified)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            if (!String.IsNullOrWhiteSpace(intent.PropertyType))
            {
                properties = properties
                    .Where(p => p.PropertyType != null && ContainsNormalized(p.PropertyType.Name, intent.PropertyType))
                    .ToList();
            }

            if (!String.IsNullOrWhiteSpace(intent.Location))
            {
                properties = properties
                    .Where(p => p.Location != null && ContainsNormalized(p.Location.Name, intent.Location))
                    .ToList();
            }

            return properties.Take(6).ToList();
        }

        private string BuildFriendlyResponse(PropertySearchIntent intent, int resultCount, bool usedGroq)
        {
            var parts = new List<string>();
            if (!String.IsNullOrWhiteSpace(intent.Purpose))
            {
                parts.Add(intent.Purpose == "Rent" ? "for rent" : "for sale");
            }

            if (!String.IsNullOrWhiteSpace(intent.PropertyType))
            {
                parts.Add(intent.PropertyType);
            }

            if (!String.IsNullOrWhiteSpace(intent.Location))
            {
                parts.Add("in " + intent.Location);
            }

            if (intent.Bedrooms.HasValue)
            {
                parts.Add(intent.Bedrooms.Value + "+ bedrooms");
            }

            if (intent.MaxPrice.HasValue)
            {
                parts.Add("under PKR " + intent.MaxPrice.Value.ToString("N0"));
            }

            var searchText = parts.Any() ? String.Join(", ", parts) : "your request";
            var source = usedGroq ? "I used AI to understand your request" : "I used local search to understand your request";

            if (resultCount == 0)
            {
                return source + ". I could not find an exact active match for " + searchText + ". Try a wider location, fewer bedrooms, or a higher budget.";
            }

            return source + ". I found " + resultCount + " active " + (resultCount == 1 ? "property" : "properties") + " matching " + searchText + ".";
        }

        private static bool ContainsAny(string text, params string[] terms)
        {
            return terms.Any(text.Contains);
        }

        private static bool ContainsPropertyType(string text, string typeName)
        {
            return ContainsNormalized(text, typeName)
                || typeName.ToLowerInvariant().Split(new[] { '/', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries).Any(text.Contains);
        }

        private static bool ContainsLocation(string text, string locationName)
        {
            return ContainsNormalized(text, locationName)
                || locationName.ToLowerInvariant().Split(new[] { '/', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries).Any(part => part.Length > 2 && text.Contains(part));
        }

        private static bool ContainsNormalized(string source, string target)
        {
            if (String.IsNullOrWhiteSpace(source) || String.IsNullOrWhiteSpace(target))
            {
                return false;
            }

            var normalizedSource = NormalizeText(source);
            var normalizedTarget = NormalizeText(target);
            return normalizedSource.Contains(normalizedTarget) || normalizedTarget.Contains(normalizedSource);
        }

        private static string NormalizeText(string value)
        {
            return Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9]+", " ").Trim();
        }

        private static int? ExtractBedrooms(string text)
        {
            var match = Regex.Match(text, @"(\d+)\s*(bed|beds|bedroom|bedrooms|br)\b");
            if (!match.Success)
            {
                return null;
            }

            int bedrooms;
            return Int32.TryParse(match.Groups[1].Value, out bedrooms) ? bedrooms : (int?)null;
        }

        private static void ApplyPriceIntent(string text, PropertySearchIntent intent)
        {
            var underMatch = Regex.Match(text, @"(under|below|max|maximum|less than)\s*(?:pkr|rs\.?|rupees)?\s*(\d+(?:\.\d+)?)\s*(crore|cr|lakh|lac|million|m)?");
            if (underMatch.Success)
            {
                intent.MaxPrice = ConvertPrice(underMatch.Groups[2].Value, underMatch.Groups[3].Value);
            }

            var aboveMatch = Regex.Match(text, @"(above|min|minimum|more than)\s*(?:pkr|rs\.?|rupees)?\s*(\d+(?:\.\d+)?)\s*(crore|cr|lakh|lac|million|m)?");
            if (aboveMatch.Success)
            {
                intent.MinPrice = ConvertPrice(aboveMatch.Groups[2].Value, aboveMatch.Groups[3].Value);
            }
        }

        private static decimal? ConvertPrice(string amountText, string unit)
        {
            decimal amount;
            if (!Decimal.TryParse(amountText, out amount))
            {
                return null;
            }

            unit = (unit ?? String.Empty).ToLowerInvariant();
            if (unit == "crore" || unit == "cr")
            {
                return amount * 10000000m;
            }

            if (unit == "lakh" || unit == "lac")
            {
                return amount * 100000m;
            }

            if (unit == "million" || unit == "m")
            {
                return amount * 1000000m;
            }

            return amount;
        }
    }
}
