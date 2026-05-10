using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VehicleX.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPurchaseHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_purchases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    purchase_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_purchases", x => x.id);
                    table.CheckConstraint("ck_customer_purchases_total_amount_non_negative", "total_amount >= 0");
                    table.ForeignKey(
                        name: "FK_customer_purchases_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_purchase_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_purchase_id = table.Column<int>(type: "integer", nullable: false),
                    part_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    part_number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_purchase_items", x => x.id);
                    table.CheckConstraint("ck_customer_purchase_items_line_total_non_negative", "line_total >= 0");
                    table.CheckConstraint("ck_customer_purchase_items_quantity_positive", "quantity > 0");
                    table.CheckConstraint("ck_customer_purchase_items_unit_price_non_negative", "unit_price >= 0");
                    table.ForeignKey(
                        name: "FK_customer_purchase_items_customer_purchases_customer_purchas~",
                        column: x => x.customer_purchase_id,
                        principalTable: "customer_purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_purchase_items_customer_purchase_id",
                table: "customer_purchase_items",
                column: "customer_purchase_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_purchases_customer_id",
                table: "customer_purchases",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_purchases_invoice_number",
                table: "customer_purchases",
                column: "invoice_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_purchase_items");

            migrationBuilder.DropTable(
                name: "customer_purchases");
        }
    }
}
