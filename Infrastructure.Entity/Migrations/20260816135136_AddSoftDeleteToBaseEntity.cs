using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "LoanHistoryEntries",
                type: "datetime2",
                nullable: true,
                comment: "Timestamp when the entity was soft-deleted");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "LoanHistoryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "User who soft-deleted the entity");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LoanHistoryEntries",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates whether the entity has been soft-deleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CatalogBooks",
                type: "datetime2",
                nullable: true,
                comment: "Timestamp when the entity was soft-deleted");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CatalogBooks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "User who soft-deleted the entity");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CatalogBooks",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates whether the entity has been soft-deleted");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistoryEntry_IsDeleted",
                table: "LoanHistoryEntries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBook_IsDeleted",
                table: "CatalogBooks",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoanHistoryEntry_IsDeleted",
                table: "LoanHistoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_CatalogBook_IsDeleted",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LoanHistoryEntries");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CatalogBooks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CatalogBooks");
        }
    }
}
