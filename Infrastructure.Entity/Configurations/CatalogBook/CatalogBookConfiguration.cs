using Infrastructure.Entity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities = Domain.Service.Entities;

namespace Infrastructure.Entity.Configurations.CatalogBook;

/// <summary>
/// Entity Framework configuration for CatalogBook entity
/// </summary>
public class CatalogBookConfiguration : BaseEntityConfiguration<Entities.CatalogBook>
{
    public override void Configure(EntityTypeBuilder<Entities.CatalogBook> builder)
    {
        // Call base configuration for audit fields (Id, CreatedAt, UpdatedAt, etc.)
        base.Configure(builder);

        // Table configuration
        builder.ToTable("CatalogBooks", tb =>
        {
            tb.HasComment("Catalog of books available in the system");
        });

        // Required string properties
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.NameLength)
            .HasComment("Book title");

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.NameLength)
            .HasComment("Book author");

        builder.Property(b => b.Isbn)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.CodeLength)
            .HasComment("International Standard Book Number");

        builder.Property(b => b.Category)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.NameLength)
            .HasComment("Book category or genre");

        builder.Property(b => b.Publisher)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.NameLength)
            .HasComment("Publisher name");

        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.DescriptionLength)
            .HasComment("Book description");

        // Integer properties
        builder.Property(b => b.PublicationYear)
            .IsRequired(false)
            .HasComment("Year of publication");

        // Optional string properties
        builder.Property(b => b.CoverColor)
            .HasMaxLength(DatabaseConstants.CodeLength)
            .IsRequired(false)
            .HasComment("Cover color of the book");

        // Unique index on ISBN
        builder.HasIndex(b => b.Isbn)
            .IsUnique()
            .HasDatabaseName("UX_CatalogBook_Isbn");

        // Indexes for frequently queried columns
        builder.HasIndex(b => b.Title)
            .HasDatabaseName("IX_CatalogBook_Title");

        builder.HasIndex(b => b.Author)
            .HasDatabaseName("IX_CatalogBook_Author");

        builder.HasIndex(b => b.Category)
            .HasDatabaseName("IX_CatalogBook_Category");

        builder.HasIndex(b => b.Publisher)
            .HasDatabaseName("IX_CatalogBook_Publisher");

        // Composite index for common queries
        builder.HasIndex(b => new { b.Category, b.Author })
            .HasDatabaseName("IX_CatalogBook_Category_Author");

        // Check constraints
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint(
                "CK_CatalogBook_PublicationYear_Valid",
                "[PublicationYear] IS NULL OR ([PublicationYear] >= 1000 AND [PublicationYear] <= YEAR(GETDATE()))");
        });
    }
}
