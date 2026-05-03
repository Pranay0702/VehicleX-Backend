using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalesManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "saswatkc123@gmail.com", "Saswat Khatry", "9808855888" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "AaryanJS@gmail.com", "Aaryan Jha Sir", "+9808855884" });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PartNumber", "UnitPrice" },
                values: new object[] { "Brembo floating disc calipers", "BRE-FDC-001", 25000m });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PartNumber" },
                values: new object[] { "BMC Air Filter", "BMC-AF-2000" });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "PartNumber" },
                values: new object[] { "LiquiMoly Engine Oil 5W-30", "LQML-OIL-530" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "ram.sharma@example.com", "Ram Sharma", "+977-9800000001" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "FullName", "PhoneNumber" },
                values: new object[] { "sita.gurung@example.com", "Sita Gurung", "+977-9800000002" });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PartNumber", "UnitPrice" },
                values: new object[] { "Brake Pad Set", "BRK-1001", 2500m });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PartNumber" },
                values: new object[] { "Air Filter", "AIR-2100" });

            migrationBuilder.UpdateData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "PartNumber" },
                values: new object[] { "Engine Oil 5W-30", "OIL-530" });
        }
    }
}
