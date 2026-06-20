using System.Web.Mvc;
using System.Web.Routing;

namespace KarachiEstateHub
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ListingDetailsMissingId",
                url: "Listings/Details",
                defaults: new { controller = "Properties", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ListingDetails",
                url: "Listings/Details/{id}",
                defaults: new { controller = "Properties", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "CreateListing",
                url: "Listings/Create",
                defaults: new { controller = "Properties", action = "Create" }
            );

            routes.MapRoute(
                name: "EditListing",
                url: "Listings/Edit/{id}",
                defaults: new { controller = "Properties", action = "Edit" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "DeleteListing",
                url: "Listings/Delete/{id}",
                defaults: new { controller = "Properties", action = "Delete" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "MyProperties",
                url: "Listings/MyProperties",
                defaults: new { controller = "Properties", action = "MyProperties" }
            );

            routes.MapRoute(
                name: "MySaved",
                url: "Listings/MySaved",
                defaults: new { controller = "Properties", action = "MySaved" }
            );

            routes.MapRoute(
                name: "SaveListing",
                url: "Listings/Save/{id}",
                defaults: new { controller = "Properties", action = "Save" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "UnsaveListing",
                url: "Listings/Unsave/{id}",
                defaults: new { controller = "Properties", action = "Unsave" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "SendListingInquiry",
                url: "Listings/SendInquiry",
                defaults: new { controller = "Properties", action = "SendInquiry" }
            );

            routes.MapRoute(
                name: "Listings",
                url: "Listings",
                defaults: new { controller = "Properties", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
