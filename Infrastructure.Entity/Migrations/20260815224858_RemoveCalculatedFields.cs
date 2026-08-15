using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCalculatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogBook_AvailableCopies_LessThanOrEqualTotal",
                table: "CatalogBooks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogBook_AvailableCopies_NonNegative",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "AvailabilityStatus",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "AvailableCopies",
                table: "CatalogBooks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailabilityStatus",
                table: "CatalogBooks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Current availability status");

            migrationBuilder.AddColumn<int>(
                name: "AvailableCopies",
                table: "CatalogBooks",
                type: "int",
                nullable: true,
                defaultValue: 0,
                comment: "Number of available copies");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogBook_AvailableCopies_LessThanOrEqualTotal",
                table: "CatalogBooks",
                sql: "[AvailableCopies] <= [TotalCopies]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogBook_AvailableCopies_NonNegative",
                table: "CatalogBooks",
                sql: "[AvailableCopies] >= 0");
        }
    }
}
