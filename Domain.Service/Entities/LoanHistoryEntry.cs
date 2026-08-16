namespace Domain.Service.Entities
{
    /// <summary>
    /// Represents a loan history entry for a book loan/return event.
    /// Inherits audit fields from BaseEntity (Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy).
    /// </summary>
    public class LoanHistoryEntry : BaseEntity
    {
        /// <summary>
        /// Book identifier. Stored as plain string (no foreign key relation).
        /// </summary>
        public string BookId { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the user who borrowed the book.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user who borrowed the book.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Mobile phone number of the user who borrowed the book.
        /// </summary>
        public string? MobilePhone { get; set; }

        /// <summary>
        /// Date when the book was loaned. Nullable if not set.
        /// </summary>
        public DateTime? LoanDate { get; set; }

        /// <summary>
        /// Date when the book was returned. Nullable if not yet returned.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Indicates whether the book has been returned.
        /// </summary>
        public bool IsReturned { get; set; }
    }
}