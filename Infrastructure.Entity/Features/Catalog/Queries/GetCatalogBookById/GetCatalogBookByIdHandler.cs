using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Domain.Service.Exceptions;
using Infrastructure.Entity.Data;
using Domain.Service.Features.Catalog.Queries.GetCatalogBookById;

namespace Infrastructure.Entity.Features.Catalog.Queries.GetCatalogBookById;

public class GetCatalogBookByIdHandler : IRequestHandler<GetCatalogBookByIdQuery, CatalogBookDto>
{
    private readonly ApplicationDbContext _db;

    public GetCatalogBookByIdHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CatalogBookDto> Handle(GetCatalogBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _db.CatalogBooks
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (book == null)
            throw new NotFoundException("CatalogBook", request.Id);

        var loanedCount = await _db.LoanHistoryEntries
            .AsNoTracking()
            .Where(l => !l.IsReturned && l.BookId == book.Id.ToString())
            .CountAsync(cancellationToken);

        var availableCopies = Math.Max(0, book.TotalCopies - loanedCount);
        var availabilityStatus = availableCopies > 0 ? "available" : "loaned";

        return new CatalogBookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Isbn = book.Isbn,
            Category = book.Category,
            Publisher = book.Publisher,
            Description = book.Description,
            PublicationYear = book.PublicationYear,
            TotalCopies = book.TotalCopies,
            CoverColor = book.CoverColor,
            AvailableCopies = availableCopies,
            AvailabilityStatus = availabilityStatus
        };
    }
}
