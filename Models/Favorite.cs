using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeSweetHome.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? PropertyId { get; set; }

        public int? DemandId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }

        [ForeignKey("DemandId")]
        public Demand? Demand { get; set; }
    }
}