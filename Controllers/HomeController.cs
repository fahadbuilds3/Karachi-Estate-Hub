using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KarachiEstateHub.Models;

namespace KarachiEstateHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly KarachiEstateDbContext db = new KarachiEstateDbContext();

        public ActionResult Index()
        {
            var featuredProperties = db.Properties
                .Include(p => p.User)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Where(p => p.Status == "Active" && p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToList();

            return View(featuredProperties);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
