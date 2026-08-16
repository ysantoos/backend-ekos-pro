using MediatR;
using Domain.Service.DTOs.Catalog;

namespace Domain.Service.Features.Catalog.Queries;

public class GetCatalogBooksQuery : IRequest<IEnumerable<CatalogBookDto>>
{
    public string? Search { get; set; }

    public string? Category { get; set; }

    public string? Author { get; set; }

    public int? PublicationYear { get; set; }

    public string? Availability { get; set; }
}
