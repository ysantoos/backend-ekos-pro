using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Microsoft.Extensions.Logging;
using Domain.Service.Exceptions;
using Domain.Service.Features.Catalog.Commands.UpdateCatalogBook;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Commands.UpdateCatalogBook;

public class UpdateCatalogBookHandler : IRequestHandler<UpdateCatalogBookCommand, CatalogBookDto>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<UpdateCatalogBookHandler> _logger;

    public UpdateCatalogBookHandler(ApplicationDbContext db, ILogger<UpdateCatalogBookHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CatalogBookDto> Handle(UpdateCatalogBookCommand request, CancellationToken cancellationToken)
    {
        // Ensure entity is tracked so EF Core detects property changes.
        // DbContext is configured with NoTracking by default for queries, so use AsTracking here.
        var entity = await _db.CatalogBooks
            .AsTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (entity == null)
            throw new NotFoundException("CatalogBook", request.Id);

        // Check ISBN uniqueness against other books
        var isbnConflict = await _db.CatalogBooks
            .AnyAsync(b => b.Isbn == request.Isbn && b.Id != request.Id, cancellationToken);

        if (isbnConflict)
        {
            _logger.LogWarning("ISBN conflict for Id={Id} Isbn={Isbn}", request.Id, request.Isbn);
            throw new BusinessException("Another book with the same ISBN already exists.");
        }

        // Update fields
        entity.Title = request.Title;
        entity.Author = request.Author;
        entity.Isbn = request.Isbn;
        entity.Category = request.Category;
        entity.Publisher = request.Publisher;
        entity.Description = request.Description;
        entity.PublicationYear = request.PublicationYear;
        entity.TotalCopies = request.TotalCopies;

        var saved = await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("UpdateCatalogBookHandler saved {Count} changes for Id={Id}", saved, request.Id);

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
