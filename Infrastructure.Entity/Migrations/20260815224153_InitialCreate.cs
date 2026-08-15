using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Book title"),
                    Author = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Book author"),
                    Isbn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "International Standard Book Number"),
                    Category = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Book category or genre"),
                    Publisher = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Publisher name"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Book description"),
                    PublicationYear = table.Column<int>(type: "int", nullable: true, comment: "Year of publication"),
                    TotalCopies = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Total number of copies"),
                    CoverColor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Cover color of the book"),
                    AvailabilityStatus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Current availability status"),
                    AvailableCopies = table.Column<int>(type: "int", nullable: true, defaultValue: 0, comment: "Number of available copies"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBooks", x => x.Id);
                    table.CheckConstraint("CK_CatalogBook_AvailableCopies_LessThanOrEqualTotal", "[AvailableCopies] <= [TotalCopies]");
                    table.CheckConstraint("CK_CatalogBook_AvailableCopies_NonNegative", "[AvailableCopies] >= 0");
                    table.CheckConstraint("CK_CatalogBook_PublicationYear_Valid", "[PublicationYear] IS NULL OR ([PublicationYear] >= 1000 AND [PublicationYear] <= YEAR(GETDATE()))");
                    table.CheckConstraint("CK_CatalogBook_TotalCopies_NonNegative", "[TotalCopies] >= 0");
                },
                comment: "Catalog of books available in the system");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_Author",
                table: "CatalogBooks",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_Category",
                table: "CatalogBooks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_Category_Author",
                table: "CatalogBooks",
                columns: new[] { "Category", "Author" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_CreatedAt",
                table: "CatalogBooks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_Publisher",
                table: "CatalogBooks",
                column: "Publisher");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_Title",
                table: "CatalogBooks",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "UX_CatalogBook_Isbn",
                table: "CatalogBooks",
                column: "Isbn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogBooks");
        }
    }
}
