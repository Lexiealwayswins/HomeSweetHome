using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeSweetHome.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Demands",
                columns: table => new
                {
                    DemandId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaxBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinBedrooms = table.Column<int>(type: "INTEGER", nullable: false),
                    MinBathrooms = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demands", x => x.DemandId);
                    table.ForeignKey(
                        name: "FK_Demands_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bedrooms = table.Column<int>(type: "INTEGER", nullable: false),
                    Bathrooms = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Images = table.Column<string>(type: "json", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Properties_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostType = table.Column<string>(type: "TEXT", nullable: false),
                    MessageText = table.Column<string>(type: "TEXT", nullable: false),
                    SentDate = table.Column<string>(type: "TEXT", nullable: false),
                    SentDateAsDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ContactMessages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId");
                    table.ForeignKey(
                        name: "FK_ContactMessages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoriteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    DemandId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.FavoriteId);
                    table.CheckConstraint("CK_Favorite_OneTarget", "[PropertyId] IS NOT NULL OR [DemandId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_Favorites_Demands_DemandId",
                        column: x => x.DemandId,
                        principalTable: "Demands",
                        principalColumn: "DemandId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Favorites_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Avatar", "Email", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, "/images/avatars/john_doe.jpg", "john@example.com", "$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "john_doe" },
                    { 2, "/images/avatars/jane_smith.jpg", "jane@example.com", "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "jane_smith" },
                    { 3, "/images/avatars/alice_wong.jpg", "alice@example.com", "$2a$11$zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz", "alice_wong" },
                    { 4, "/images/avatars/bob_lee.jpg", "bob@example.com", "$2a$11$wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww", "bob_lee" }
                });

            migrationBuilder.InsertData(
                table: "ContactMessages",
                columns: new[] { "MessageId", "MessageText", "PostId", "PostType", "PropertyId", "ReceiverId", "SenderId", "SentDate", "SentDateAsDateTime" },
                values: new object[] { 1, "Is this still available?", 1, "Property", null, 1, 2, "2025-03-14 00:00:00", new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Demands",
                columns: new[] { "DemandId", "CreatedAt", "Description", "Location", "MaxBudget", "MinBathrooms", "MinBedrooms", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Looking for a quiet apartment with good transit access.", "Downtown Vancouver", 1500.00m, 1, 2, "Seeking a 2-Bedroom Downtown", 2 },
                    { 2, new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Looking for a house with a backyard.", "Burnaby", 2500.00m, 2, 3, "Need a Family Home", 2 },
                    { 3, new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Affordable studio near downtown.", "Victoria", 1000.00m, 1, 1, "Looking for a Studio", 4 },
                    { 4, new DateTime(2025, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Modern condo with amenities.", "Richmond", 1800.00m, 1, 2, "2-Bedroom Condo Wanted", 4 }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "PropertyId", "Bathrooms", "Bedrooms", "CreatedAt", "Description", "Images", "Location", "Price", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 2, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "A nice 2-bedroom apartment", "[\"/images/properties/cozy_apartment1.jpg\",\"/images/properties/cozy_apartment2.jpg\"]", "Downtown Vancouver", 1200.00m, "Cozy Apartment", 1 },
                    { 2, 2, 3, new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bright and spacious condo with a view", "[\"/images/properties/sunny_condo1.jpg\"]", "Victoria", 1500.00m, "Sunny Condo", 1 },
                    { 3, 1, 1, new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stylish loft in the heart of the city", "[\"/images/properties/modern_loft1.jpg\"]", "Surrey", 1800.00m, "Modern Loft", 3 },
                    { 4, 3, 4, new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Perfect for a small family", "[\"/images/properties/family_house1.jpg\"]", "Burnaby", 2000.00m, "Family House", 3 },
                    { 5, 1, 1, new DateTime(2025, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compact and affordable studio", "[\"/images/properties/cozy_studio1.jpg\"]", "Richmond", 900.00m, "Cozy Studio", 1 }
                });

            migrationBuilder.InsertData(
                table: "Favorites",
                columns: new[] { "FavoriteId", "CreatedAt", "DemandId", "PropertyId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 2 },
                    { 2, new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_PropertyId",
                table: "ContactMessages",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_ReceiverId",
                table: "ContactMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_SenderId",
                table: "ContactMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Demands_UserId",
                table: "Demands",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_DemandId",
                table: "Favorites",
                column: "DemandId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_PropertyId",
                table: "Favorites",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Demands");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
