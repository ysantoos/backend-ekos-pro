using MediatR;

namespace Domain.Service.Features.Catalog.Commands.DeleteCatalogBook;

public class DeleteCatalogBookCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
