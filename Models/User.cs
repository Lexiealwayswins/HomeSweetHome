using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeSweetHome.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string? Avatar { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<ContactMessage> SentMessages { get; set; } = new List<ContactMessage>();
        public ICollection<ContactMessage> ReceivedMessages { get; set; } = new List<ContactMessage>();
        public ICollection<Demand> Demands { get; set; } = new List<Demand>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}