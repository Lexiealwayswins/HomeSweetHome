using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeSweetHome.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Location { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public int UserId { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ICollection<ContactMessage> Messages { get; set; } = new List<ContactMessage>();

        public DateTime CreatedAt { get; set; }
    }
}