using Infrastructure.Entity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities = Domain.Service.Entities;

namespace Infrastructure.Entity.Configurations.LoanHistory;

/// <summary>
/// EF Core configuration for LoanHistoryEntry entity.
/// Note: BookId is a plain string and intentionally not configured as a foreign key.
/// </summary>
public class LoanHistoryEntryConfiguration : BaseEntityConfiguration<Entities.LoanHistoryEntry>
{
    public override void Configure(EntityTypeBuilder<Entities.LoanHistoryEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanHistoryEntries", tb => tb.HasComment("Historical records of book loans and returns"));

        builder.Property(x => x.BookId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.CodeLength)
            .HasComment("Reference id of the book (no FK)");

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.NameLength)
            .HasComment("Full name of the user who borrowed the book");

        builder.Property(x => x.Email)
            .IsRequired(false)
            .HasMaxLength(DatabaseConstants.EmailLength)
            .HasComment("Email of the user who borrowed the book");

        builder.Property(x => x.MobilePhone)
            .IsRequired(false)
            .HasMaxLength(DatabaseConstants.PhoneLength)
            .HasComment("Mobile phone number of the user who borrowed the book");

        builder.Property(x => x.LoanDate)
            .IsRequired(false)
            .HasComment("Date when the book was loaned");

        builder.Property(x => x.ReturnDate)
            .IsRequired(false)
            .HasComment("Date when the book was returned");

        builder.Property(x => x.IsReturned)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Whether the book has been returned");

        // Indexes for common queries
        builder.HasIndex(x => x.BookId).HasDatabaseName("IX_LoanHistory_BookId");
        builder.HasIndex(x => x.FullName).HasDatabaseName("IX_LoanHistory_FullName");
        builder.HasIndex(x => x.LoanDate).HasDatabaseName("IX_LoanHistory_LoanDate");
    }
}
