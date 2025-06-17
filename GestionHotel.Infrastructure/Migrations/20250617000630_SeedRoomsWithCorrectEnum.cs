using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionHotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoomsWithCorrectEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "Condition", "Number", "Price", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), 1, 0, "101", 60m, 0 },
                    { new Guid("11111111-1111-1111-1111-111111111102"), 1, 0, "102", 65m, 0 },
                    { new Guid("11111111-1111-1111-1111-111111111103"), 2, 3, "103", 80m, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111104"), 2, 3, "104", 85m, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111105"), 2, 1, "105", 90m, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111106"), 3, 0, "106", 110m, 2 },
                    { new Guid("11111111-1111-1111-1111-111111111107"), 3, 3, "107", 115m, 2 },
                    { new Guid("11111111-1111-1111-1111-111111111108"), 4, 1, "108", 140m, 2 },
                    { new Guid("11111111-1111-1111-1111-111111111109"), 4, 3, "109", 145m, 2 },
                    { new Guid("11111111-1111-1111-1111-111111111110"), 4, 0, "110", 150m, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111105"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111106"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111109"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111110"));
        }
    }
}
