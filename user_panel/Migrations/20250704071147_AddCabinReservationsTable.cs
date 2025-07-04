using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace user_panel.Migrations
{
    /// <inheritdoc />
    public partial class AddCabinReservationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "cab",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.CreateTable(
                name: "CabinReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CabinReservations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabinReservations_ApplicationUserId",
                table: "CabinReservations",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabinReservations");

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CabinId = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_cab_CabinId",
                        column: x => x.CabinId,
                        principalTable: "cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "cab",
                columns: new[] { "Id", "Description", "Location", "PricePerHour" },
                values: new object[,]
                {
                    { 1, "A modern, compact cabin perfect for a high-intensity workout. Equipped with a smart treadmill and weight set.", "Bornova/İzmir", 25.00m },
                    { 2, "Spacious cabin with a focus on yoga and flexibility. Includes a full-length mirror and yoga mats.", "Karşıyaka/İzmir", 25.00m },
                    { 3, "Premium cabin featuring a rowing machine and advanced monitoring systems for performance tracking.", "Çankaya/Ankara", 30.00m },
                    { 4, "Standard cabin with essential cardio and strength training equipment. Great for a balanced workout.", "Akyurt/Ankara", 20.00m },
                    { 5, "An urban-style cabin with a boxing bag and a high-performance stationary bike.", "Beşiktaş/İstanbul", 35.00m },
                    { 6, "Historic district cabin offering a quiet and serene environment for mindful exercise and meditation.", "Fatih/İstanbul", 30.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApplicationUserId",
                table: "Bookings",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CabinId",
                table: "Bookings",
                column: "CabinId");
        }
    }
}
