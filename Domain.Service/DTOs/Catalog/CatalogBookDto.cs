namespace Domain.Service.DTOs.Catalog;

public class CatalogBookDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int? PublicationYear { get; set; }

    public int TotalCopies { get; set; }

    public string? CoverColor { get; set; }

    public string AvailabilityStatus { get; set; } = string.Empty;

    public int AvailableCopies { get; set; }
}
