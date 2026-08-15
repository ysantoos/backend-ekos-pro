using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTotalCopiesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogBook_TotalCopies_NonNegative",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "TotalCopies",
                table: "CatalogBooks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCopies",
                table: "CatalogBooks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Total number of copies");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogBook_TotalCopies_NonNegative",
                table: "CatalogBooks",
                sql: "[TotalCopies] >= 0");
        }
    }
}
