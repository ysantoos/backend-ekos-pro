using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Domain.Service.Exceptions;
using Domain.Service.Features.Catalog.Commands.UpdateCatalogBook;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Commands.UpdateCatalogBook;

public class UpdateCatalogBookHandler : IRequestHandler<UpdateCatalogBookCommand, CatalogBookDto>
{
    private readonly ApplicationDbContext _db;

    public UpdateCatalogBookHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CatalogBookDto> Handle(UpdateCatalogBookCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.CatalogBooks.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (entity == null)
            throw new NotFoundException("CatalogBook", request.Id);

        // Check ISBN uniqueness against other books
        var isbnConflict = await _db.CatalogBooks
            .AnyAsync(b => b.Isbn == request.Isbn && b.Id != request.Id, cancellationToken);

        if (isbnConflict)
            throw new BusinessException("Another book with the same ISBN already exists.");

        // Update fields
        entity.Title = request.Title;
        entity.Author = request.Author;
        entity.Isbn = request.Isbn;
        entity.Category = request.Category;
        entity.Publisher = request.Publisher;
        entity.Description = request.Description;
        entity.PublicationYear = request.PublicationYear;
        entity.TotalCopies = request.TotalCopies;
        entity.CoverColor = request.CoverColor;

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
            AvailabilityStatus = entity.TotalCopies > 0 ? "available" : "loaned",
            AvailableCopies = entity.TotalCopies // recalculated elsewhere
        };
    }
}
