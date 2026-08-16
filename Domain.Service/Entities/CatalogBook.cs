namespace Domain.Service.Entities
{
    public class CatalogBook : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Isbn { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int? PublicationYear { get; set; }

        public int TotalCopies { get; set; }

        public string? CoverColor { get; set; }
    }
}
