using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanHistoryEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Reference id of the book (no FK)"),
                    UserName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Name of the user who borrowed the book"),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Date when the book was loaned"),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Date when the book was returned"),
                    IsReturned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Whether the book has been returned"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanHistoryEntries", x => x.Id);
                },
                comment: "Historical records of book loans and returns");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_BookId",
                table: "LoanHistoryEntries",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_LoanDate",
                table: "LoanHistoryEntries",
                column: "LoanDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_UserName",
                table: "LoanHistoryEntries",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistoryEntry_CreatedAt",
                table: "LoanHistoryEntries",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanHistoryEntries");
        }
    }
}
