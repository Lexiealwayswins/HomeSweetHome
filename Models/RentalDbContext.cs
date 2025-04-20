using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeSweetHome.Models
{
    public class RentalDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Demand> Demands { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Rental.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed initial data for User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "john_doe",
                    Email = "john@example.com",
                    PasswordHash = "$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", // 替换为实际哈希值
                    Avatar = "/images/avatars/john_doe.jpg"
                },
                new User
                {
                    UserId = 2,
                    Username = "jane_smith",
                    Email = "jane@example.com",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", // 替换为实际哈希值
                    Avatar = "/images/avatars/jane_smith.jpg"
                },
                new User
                {
                    UserId = 3,
                    Username = "alice_wong",
                    Email = "alice@example.com",
                    PasswordHash = "$2a$11$zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz", // 替换为实际哈希值
                    Avatar = "/images/avatars/alice_wong.jpg"
                },
                new User
                {
                    UserId = 4,
                    Username = "bob_lee",
                    Email = "bob@example.com",
                    PasswordHash = "$2a$11$wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww", // 替换为实际哈希值
                    Avatar = "/images/avatars/bob_lee.jpg"
                }
            );

            // Seed initial data for Property
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    PropertyId = 1,
                    Title = "Cozy Apartment",
                    Description = "A nice 2-bedroom apartment",
                    Location = "Downtown Vancouver",
                    Price = 1200.00M,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    UserId = 1,
                    Images = new List<string> { "/images/properties/cozy_apartment1.jpg", "/images/properties/cozy_apartment2.jpg" },
                    CreatedAt = new DateTime(2025, 3, 10, 0, 0, 0)
                },
                new Property
                {
                    PropertyId = 2,
                    Title = "Sunny Condo",
                    Description = "Bright and spacious condo with a view",
                    Location = "Victoria",
                    Price = 1500.00M,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    UserId = 1,
                    Images = new List<string> { "/images/properties/sunny_condo1.jpg" },
                    CreatedAt = new DateTime(2025, 2, 16, 0, 0, 0)
                },
                new Property
                {
                    PropertyId = 3,
                    Title = "Modern Loft",
                    Description = "Stylish loft in the heart of the city",
                    Location = "Surrey",
                    Price = 1800.00M,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    UserId = 3,
                    Images = new List<string> { "/images/properties/modern_loft1.jpg" },
                    CreatedAt = new DateTime(2025, 4, 16, 0, 0, 0)
                },
                new Property
                {
                    PropertyId = 4,
                    Title = "Family House",
                    Description = "Perfect for a small family",
                    Location = "Burnaby",
                    Price = 2000.00M,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    UserId = 3,
                    Images = new List<string> { "/images/properties/family_house1.jpg" },
                    CreatedAt = new DateTime(2025, 3, 18, 0, 0, 0)
                },
                new Property
                {
                    PropertyId = 5,
                    Title = "Cozy Studio",
                    Description = "Compact and affordable studio",
                    Location = "Richmond",
                    Price = 900.00M,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    UserId = 1,
                    Images = new List<string> { "/images/properties/cozy_studio1.jpg" },
                    CreatedAt = new DateTime(2025, 3, 16, 0, 0, 0)
                }
            );

            // Seed initial data for ContactMessage
            modelBuilder.Entity<ContactMessage>().HasData(
                new ContactMessage
                {
                    MessageId = 1,
                    SenderId = 2,
                    ReceiverId = 1,
                    PostId = 1, 
                    PostType = "Property", 
                    MessageText = "Is this still available?",
                    SentDate = new DateTime(2025, 3, 14, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") // 转换为字符串
                }
            );

            // Seed initial data for Demand
            modelBuilder.Entity<Demand>().HasData(
                new Demand
                {
                    DemandId = 1,
                    UserId = 2,
                    Title = "Seeking a 2-Bedroom Downtown",
                    Description = "Looking for a quiet apartment with good transit access.",
                    Location = "Downtown Vancouver",
                    MaxBudget = 1500.00M,
                    MinBedrooms = 2,
                    MinBathrooms = 1,
                    CreatedAt = new DateTime(2025, 3, 16, 0, 0, 0)
                },
                new Demand
                {
                    DemandId = 2,
                    UserId = 2,
                    Title = "Need a Family Home",
                    Description = "Looking for a house with a backyard.",
                    Location = "Burnaby",
                    MaxBudget = 2500.00M,
                    MinBedrooms = 3,
                    MinBathrooms = 2,
                    CreatedAt = new DateTime(2025, 3, 17, 0, 0, 0)
                },
                new Demand
                {
                    DemandId = 3,
                    UserId = 4,
                    Title = "Looking for a Studio",
                    Description = "Affordable studio near downtown.",
                    Location = "Victoria",
                    MaxBudget = 1000.00M,
                    MinBedrooms = 1,
                    MinBathrooms = 1,
                    CreatedAt = new DateTime(2025, 3, 18, 0, 0, 0)
                },
                new Demand
                {
                    DemandId = 4,
                    UserId = 4,
                    Title = "2-Bedroom Condo Wanted",
                    Description = "Modern condo with amenities.",
                    Location = "Richmond",
                    MaxBudget = 1800.00M,
                    MinBedrooms = 2,
                    MinBathrooms = 1,
                    CreatedAt = new DateTime(2025, 3, 19, 0, 0, 0)
                }
            );

            // Seed initial data for Favorite
            modelBuilder.Entity<Favorite>().HasData(
                new Favorite
                {
                    FavoriteId = 1,
                    UserId = 2,
                    PropertyId = 1,
                    CreatedAt = new DateTime(2025, 3, 17, 0, 0, 0)
                },
                new Favorite
                {
                    FavoriteId = 2,
                    UserId = 1,
                    DemandId = 1,
                    CreatedAt = new DateTime(2025, 3, 17, 0, 0, 0)
                }
            );

            // 明确指定 ContactMessage 的主键
            modelBuilder.Entity<ContactMessage>()
                .HasKey(cm => cm.MessageId);

            // 配置 ContactMessage 和 User (Receiver) 的关系
            modelBuilder.Entity<ContactMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置 ContactMessage 和 User (Sender) 的关系
            modelBuilder.Entity<ContactMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置 Demand 和 User 的关系
            modelBuilder.Entity<Demand>()
                .HasOne(d => d.User)
                .WithMany(u => u.Demands)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置 Favorite 和 User 的关系
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置 Favorite 和 Property 的关系
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Property)
                .WithMany()
                .HasForeignKey(f => f.PropertyId)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置 Favorite 和 Demand 的关系
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Demand)
                .WithMany()
                .HasForeignKey(f => f.DemandId)
                .OnDelete(DeleteBehavior.SetNull);

            // 添加约束：PropertyId 和 DemandId 不能同时为空
            modelBuilder.Entity<Favorite>()
                .ToTable(t => t.HasCheckConstraint("CK_Favorite_OneTarget", "[PropertyId] IS NOT NULL OR [DemandId] IS NOT NULL"));

            // 配置 Property 的 Images 字段为 JSON
            modelBuilder.Entity<Property>()
                .Property(p => p.Images)
                .HasColumnType("json");

            // 配置 Property 和 User 的关系
            modelBuilder.Entity<Property>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}