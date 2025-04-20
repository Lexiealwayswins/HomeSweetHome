using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeSweetHome.Models
{
    public class Demand
    {
        [Key]
        public int DemandId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Location { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxBudget { get; set; }

        [Required]
        public int MinBedrooms { get; set; }

        [Required]
        public int MinBathrooms { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}