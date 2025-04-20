using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeSweetHome.Models;
using System.Diagnostics;

namespace HomeSweetHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly RentalDbContext _context;

        public HomeController(RentalDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string viewMode, string location, decimal? maxPrice, int? bedrooms, int? bathrooms)
        {
            viewMode = viewMode ?? "properties";
            ViewBag.ViewMode = viewMode;

            int userId = 0;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            ViewBag.Favorites = userId != 0
                ? _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.PropertyId != null ? f.PropertyId.Value : f.DemandId!.Value)
                    .ToList()
                : new List<int>();

            IQueryable<object> items = viewMode == "properties"
                ? _context.Properties
                    .Where(p => location == null || p.Location.Contains(location))
                    .Where(p => maxPrice == null || p.Price <= maxPrice)
                    .Where(p => bedrooms == null || p.Bedrooms >= bedrooms)
                    .Where(p => bathrooms == null || p.Bathrooms >= bathrooms)
                    .Cast<object>()
                : _context.Demands
                    .Include(d => d.User)
                    .Where(d => location == null || d.Location.Contains(location))
                    .Where(d => maxPrice == null || d.MaxBudget <= maxPrice)
                    .Where(d => bedrooms == null || d.MinBedrooms >= bedrooms)
                    .Where(d => bathrooms == null || d.MinBathrooms >= bathrooms)
                    .Cast<object>();

            return View(items.ToList());
        }

        public IActionResult PropertyDetail(int id)
        {
            var property = _context.Properties
                .Include(p => p.User)
                .FirstOrDefault(p => p.PropertyId == id);

            if (property == null)
            {
                return NotFound();
            }

            int userId = 0;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            ViewBag.IsOwner = userId != 0 && property.UserId == userId;
            return View(property);
        }

        public IActionResult DemandDetail(int id)
        {
            var demand = _context.Demands
                .Include(d => d.User)
                .FirstOrDefault(d => d.DemandId == id);

            if (demand == null)
            {
                return NotFound();
            }

            return View(demand);
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int id, string type, bool add)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized();
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            if (type == "property")
            {
                var favorite = _context.Favorites
                    .FirstOrDefault(f => f.UserId == userId && f.PropertyId == id);

                if (add && favorite == null)
                {
                    _context.Favorites.Add(new Favorite
                    {
                        UserId = userId,
                        PropertyId = id,
                        CreatedAt = DateTime.Now
                    });
                }
                else if (!add && favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                }
            }
            else if (type == "demand")
            {
                var favorite = _context.Favorites
                    .FirstOrDefault(f => f.UserId == userId && f.DemandId == id);

                if (add && favorite == null)
                {
                    _context.Favorites.Add(new Favorite
                    {
                        UserId = userId,
                        DemandId = id,
                        CreatedAt = DateTime.Now
                    });
                }
                else if (!add && favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                }
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}