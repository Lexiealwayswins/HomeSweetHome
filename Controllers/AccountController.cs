using Microsoft.AspNetCore.Mvc;
using HomeSweetHome.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HomeSweetHome.Controllers
{
    public class AccountController : Controller
    {
        private readonly RentalDbContext _context;

        public AccountController(RentalDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Avatar", user.Avatar ?? "/images/default-avatar.png"),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, IFormFile? AvatarFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Error = "Please fill in all required fields correctly. Errors: " + string.Join(", ", errors);
                return View(user);
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email is already registered.";
                return View(user);
            }

            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            else
            {
                ViewBag.Error = "Password is required.";
                return View(user);
            }

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }

                user.Avatar = $"/images/avatars/{fileName}";
            }
            else
            {
                user.Avatar = "/images/default-avatar.png";
            }

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to register: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.Error += $" Inner Exception: {ex.InnerException.Message}";
                }
                return View(user);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MyFavorites()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var favorites = _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Property)
                .Include(f => f.Demand)
                .ThenInclude(d => d!.User)
                .ToList();

            return View(favorites);
        }

        public IActionResult MyProperties()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var properties = _context.Properties
                .Where(p => p.UserId == userId)
                .ToList();

            return View(properties);
        }

        public IActionResult MyDemands()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var demands = _context.Demands
                .Where(d => d.UserId == userId)
                .Include(d => d.User)
                .ToList();

            return View(demands);
        }

        public IActionResult MyProfile()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UploadImages(List<IFormFile> imageFiles)
        {
            if (imageFiles == null || !imageFiles.Any())
            {
                return Json(new { success = false, error = "No images selected." });
            }

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/properties");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var imagePaths = new List<string>();
            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    imagePaths.Add($"/images/properties/{fileName}");
                }
            }

            return Json(new { success = true, imagePaths });
        }

        [HttpPost]
        public IActionResult RemoveImage(string imagePath)
        {
            try
            {
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateProperty(Property property)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            property.UserId = userId;
            property.CreatedAt = DateTime.UtcNow;

            if (property.Images == null || !property.Images.Any())
            {
                property.Images = new List<string> { "/images/placeholder.jpg" };
            }

            try
            {
                _context.Properties.Add(property);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to create property: {ex.Message}";
                return View("PropertyPosts", property);
            }

            return RedirectToAction("MyProperties", "Account");
        }

        [HttpPost]
        public IActionResult EditProperty(Property property)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var existingProperty = _context.Properties
                .FirstOrDefault(p => p.PropertyId == property.PropertyId && p.UserId == userId);
            if (existingProperty == null)
            {
                TempData["ErrorMessage"] = "Property not found or you do not have permission to edit it.";
                return View("PropertyPosts", property);
            }

            existingProperty.Title = property.Title;
            existingProperty.Description = property.Description;
            existingProperty.Location = property.Location;
            existingProperty.Price = property.Price;
            existingProperty.Bedrooms = property.Bedrooms;
            existingProperty.Bathrooms = property.Bathrooms;
            existingProperty.Images = property.Images;

            if (existingProperty.Images == null || !existingProperty.Images.Any())
            {
                existingProperty.Images = new List<string> { "/images/placeholder.jpg" };
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update property: {ex.Message}";
                return View("PropertyPosts", property);
            }

            return RedirectToAction("MyProperties", "Account");
        }


        [HttpPost]
        public IActionResult DeleteProperty(int id)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var property = _context.Properties
                .FirstOrDefault(p => p.PropertyId == id && p.UserId == userId);

            if (property == null)
            {
                return NotFound();
            }

            _context.Properties.Remove(property);
            _context.SaveChanges();
            return RedirectToAction("MyProperties", "Account");
        }

        [HttpPost]
        public IActionResult CreateDemand(Demand demand)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            demand.UserId = userId;
            demand.CreatedAt = DateTime.Now;

            _context.Demands.Add(demand);
            _context.SaveChanges();
            return RedirectToAction("MyDemands", "Account");
        }

        [HttpPost]
        public IActionResult EditDemand(Demand demand)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var existingDemand = _context.Demands
                .FirstOrDefault(d => d.DemandId == demand.DemandId && d.UserId == userId);
            if (existingDemand == null)
            {
                return NotFound();
            }

            existingDemand.Title = demand.Title;
            existingDemand.Description = demand.Description;
            existingDemand.Location = demand.Location;
            existingDemand.MaxBudget = demand.MaxBudget;
            existingDemand.MinBedrooms = demand.MinBedrooms;
            existingDemand.MinBathrooms = demand.MinBathrooms;

            _context.SaveChanges();
            return RedirectToAction("MyDemands", "Account");
        }

        [HttpPost]
        public IActionResult DeleteDemand(int id)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var demand = _context.Demands
                .FirstOrDefault(d => d.DemandId == id && d.UserId == userId);

            if (demand == null)
            {
                return NotFound();
            }

            _context.Demands.Remove(demand);
            _context.SaveChanges();
            return RedirectToAction("MyDemands", "Account");
        }

        [HttpPost]
        public IActionResult UpdateProfile(User user, IFormFile? AvatarFile, string? CurrentPassword, string? Password, string? ConfirmPassword)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingUser = _context.Users.Find(user.UserId);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("MyProfile", "Account");
            }

            if (string.IsNullOrWhiteSpace(CurrentPassword) || !BCrypt.Net.BCrypt.Verify(CurrentPassword, existingUser.PasswordHash))
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return View("MyProfile", existingUser);
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                if (Password != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "New password and confirm password do not match.";
                    return View("MyProfile", existingUser);
                }
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                TempData["ErrorMessage"] = "Username cannot be empty.";
                return View("MyProfile", existingUser);
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                TempData["ErrorMessage"] = "Email cannot be empty.";
                return View("MyProfile", existingUser);
            }

            if (_context.Users.Any(u => u.Email == user.Email && u.UserId != user.UserId))
            {
                TempData["ErrorMessage"] = "This email is already in use by another user.";
                return View("MyProfile", existingUser);
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }

                existingUser.Avatar = $"/images/avatars/{fileName}";
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update profile: {ex.Message}";
                return View("MyProfile", existingUser);
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("MyProfile", "Account");
        }

        public IActionResult PropertyPosts(int id = 0)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var property = id == 0
                ? new Property()
                : _context.Properties.Find(id) ?? new Property();

            return View(property);
        }

        public IActionResult DemandPosts(int id = 0)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var demand = id == 0
                ? new Demand()
                : _context.Demands.Find(id) ?? new Demand();

            return View(demand);
        }

        public IActionResult MyMessages(int? receiverId = null, int? postId = null, string? postType = null)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var conversationsQuery = _context.ContactMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    ContactId = g.Key,
                    Messages = g.ToList()
                });

            var conversationList = conversationsQuery.ToList().Select(c => new
            {
                ContactId = c.ContactId,
                LatestMessageId = c.Messages
                    .OrderByDescending(m => DateTime.Parse(m.SentDate))
                    .Select(m => m.MessageId)
                    .First(),
                PostId = c.Messages.First().PostId,
                PostType = c.Messages.First().PostType
            })
            .OrderByDescending(c => c.LatestMessageId)
            .ToList();

            var latestMessageIds = conversationList.Select(c => c.LatestMessageId).ToList();
            var latestMessages = _context.ContactMessages
                .Where(m => latestMessageIds.Contains(m.MessageId))
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToList();

            var contactIds = conversationList.Select(c => c.ContactId).ToList();
            var contacts = _context.Users
                .Where(u => contactIds.Contains(u.UserId))
                .ToList();

            var propertyIds = conversationList.Where(c => c.PostType == "Property").Select(c => c.PostId).ToList();
            var demandIds = conversationList.Where(c => c.PostType == "Demand").Select(c => c.PostId).ToList();
            var properties = _context.Properties
                .Where(p => propertyIds.Contains(p.PropertyId))
                .ToDictionary(p => p.PropertyId, p => p.Title);
            var demands = _context.Demands
                .Where(d => demandIds.Contains(d.DemandId))
                .ToDictionary(d => d.DemandId, d => d.Title);

            var conversations = conversationList.Select(c =>
            {
                var latestMessage = latestMessages.FirstOrDefault(m => m.MessageId == c.LatestMessageId);
                var contact = contacts.FirstOrDefault(u => u.UserId == c.ContactId);
                string postTitle = c.PostType == "Property"
                    ? properties.GetValueOrDefault(c.PostId, "Unknown Property")
                    : demands.GetValueOrDefault(c.PostId, "Unknown Demand");

                return new
                {
                    ContactId = c.ContactId,
                    Contact = contact,
                    LatestMessage = latestMessage,
                    PostId = c.PostId,
                    PostType = c.PostType,
                    PostTitle = postTitle
                };
            }).ToList();

            ViewBag.HasConversations = conversations.Any();

            List<ContactMessage> messages = new();
            User? selectedContact = null;
            dynamic? selectedPost = null;
            if (receiverId.HasValue)
            {
                messages = _context.ContactMessages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == userId))
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .AsEnumerable()
                    .OrderBy(m => DateTime.Parse(m.SentDate))
                    .ToList();

                selectedContact = _context.Users.Find(receiverId);
                if (selectedContact == null)
                {
                    return NotFound("Selected contact not found.");
                }

                if (messages.Any())
                {
                    var firstMessage = messages.First();
                    if (firstMessage.PostType == "Property")
                    {
                        selectedPost = _context.Properties.FirstOrDefault(p => p.PropertyId == firstMessage.PostId);
                    }
                    else if (firstMessage.PostType == "Demand")
                    {
                        selectedPost = _context.Demands.FirstOrDefault(d => d.DemandId == firstMessage.PostId);
                    }
                }
            }

            ViewBag.UserId = userId;
            ViewBag.Conversations = conversations;
            ViewBag.Messages = messages;
            ViewBag.SelectedContact = selectedContact;
            ViewBag.SelectedPost = selectedPost;
            ViewBag.PostId = postId;
            ViewBag.PostType = postType;

            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(int receiverId, int postId, string postType, string content)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "Invalid user session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var sender = _context.Users.Find(userId);
            var receiver = _context.Users.Find(receiverId);

            if (sender == null)
            {
                TempData["ErrorMessage"] = "Sender does not exist.";
                return RedirectToAction("MyMessages", new { receiverId, postId, postType });
            }

            if (receiver == null)
            {
                TempData["ErrorMessage"] = "Receiver does not exist.";
                return RedirectToAction("MyMessages", new { receiverId, postId, postType });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Message content cannot be empty.";
                return RedirectToAction("MyMessages", new { receiverId, postId, postType });
            }

            var message = new ContactMessage
            {
                SenderId = userId,
                ReceiverId = receiverId,
                PostId = postId,
                PostType = postType,
                MessageText = content,
                SentDateAsDateTime = DateTime.UtcNow
            };

            try
            {
                _context.ContactMessages.Add(message);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send message: {ex.Message}";
                return RedirectToAction("MyMessages", new { receiverId, postId, postType });
            }

            return RedirectToAction("MyMessages", new { receiverId, postId, postType });
        }
    }
}