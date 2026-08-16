using MediatR;
using Domain.Service.DTOs.Catalog;

namespace Domain.Service.Features.Catalog.Queries.GetCatalogBookById;

public class GetCatalogBookByIdQuery : IRequest<CatalogBookDto>
{
    public Guid Id { get; set; }
}
