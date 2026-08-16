using MediatR;
using Domain.Service.DTOs.Catalog;

namespace Domain.Service.Features.Catalog.Queries;

public class GetCatalogBooksQuery : IRequest<CatalogPageResponseDto>
{
    public string? Search { get; set; }

    public string? Category { get; set; }

    public string? Author { get; set; }

    public int? PublicationYear { get; set; }

    public string? Availability { get; set; }

    // Pagination
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
