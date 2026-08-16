using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Domain.Service.Exceptions;
using Domain.Service.Features.Catalog.Commands.CreateCatalogBook;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Commands.CreateCatalogBook;

public class CreateCatalogBookHandler : IRequestHandler<CreateCatalogBookCommand, CatalogBookDto>
{
    private readonly ApplicationDbContext _db;

    public CreateCatalogBookHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CatalogBookDto> Handle(CreateCatalogBookCommand request, CancellationToken cancellationToken)
    {
        // Check ISBN uniqueness (exclude soft-deleted)
        var exists = await _db.CatalogBooks.AnyAsync(b => b.Isbn == request.Isbn, cancellationToken);
        if (exists)
            throw new BusinessException("A book with the same ISBN already exists.");

        var entity = new Domain.Service.Entities.CatalogBook
        {
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            Category = request.Category,
            Publisher = request.Publisher,
            Description = request.Description,
            PublicationYear = request.PublicationYear,
            TotalCopies = request.TotalCopies,
            CoverColor = request.CoverColor
        };

        _db.CatalogBooks.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new CatalogBookDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Author = entity.Author,
            Isbn = entity.Isbn,
            Category = entity.Category,
            Publisher = entity.Publisher,
            Description = entity.Description,
            PublicationYear = entity.PublicationYear,
            TotalCopies = entity.TotalCopies,
            CoverColor = entity.CoverColor,
            AvailabilityStatus = "available",
            AvailableCopies = entity.TotalCopies
        };
    }
}
