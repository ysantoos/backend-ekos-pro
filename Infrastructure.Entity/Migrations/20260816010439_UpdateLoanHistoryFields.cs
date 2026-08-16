using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanHistoryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoanHistory_UserName",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "LoanHistoryEntries");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "LoanHistoryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Email of the user who borrowed the book");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "LoanHistoryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Full name of the user who borrowed the book");

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "LoanHistoryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Mobile phone number of the user who borrowed the book");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_FullName",
                table: "LoanHistoryEntries",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoanHistory_FullName",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "LoanHistoryEntries");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "LoanHistoryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Name of the user who borrowed the book");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_UserName",
                table: "LoanHistoryEntries",
                column: "UserName");
        }
    }
}
