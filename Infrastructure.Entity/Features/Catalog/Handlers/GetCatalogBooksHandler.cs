using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Domain.Service.Features.Catalog.Queries;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Handlers;

public class GetCatalogBooksHandler : IRequestHandler<GetCatalogBooksQuery, IEnumerable<CatalogBookDto>>
{
    private readonly ApplicationDbContext _db;

    public GetCatalogBooksHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CatalogBookDto>> Handle(GetCatalogBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _db.CatalogBooks.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(b => b.Title.Contains(s) || b.Author.Contains(s) || b.Description.Contains(s) || b.Isbn.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var c = request.Category.Trim();
            query = query.Where(b => b.Category == c);
        }

        if (!string.IsNullOrWhiteSpace(request.Author))
        {
            var a = request.Author.Trim();
            query = query.Where(b => b.Author == a);
        }

        if (request.PublicationYear.HasValue)
        {
            query = query.Where(b => b.PublicationYear == request.PublicationYear.Value);
        }

        var books = await query
            .Select(b => new CatalogBookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Isbn = b.Isbn,
                Category = b.Category,
                Publisher = b.Publisher,
                Description = b.Description,
                PublicationYear = b.PublicationYear,
                TotalCopies = b.TotalCopies,
                CoverColor = b.CoverColor,
                AvailabilityStatus = "available",
                AvailableCopies = b.TotalCopies
            })
            .ToListAsync(cancellationToken);

        if (books.Count == 0)
            return books;

        var loanedCounts = await _db.LoanHistoryEntries
            .AsNoTracking()
            .Where(l => !l.IsReturned)
            .GroupBy(l => l.BookId)
            .Select(g => new { BookId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BookId, x => x.Count, cancellationToken);

        foreach (var book in books)
        {
            var idStr = book.Id.ToString();
            loanedCounts.TryGetValue(idStr, out var loaned);
            var available = Math.Max(0, book.TotalCopies - loaned);
            book.AvailableCopies = available;
            book.AvailabilityStatus = available > 0 ? "available" : "loaned";
        }

        if (!string.IsNullOrWhiteSpace(request.Availability))
        {
            var av = request.Availability.Trim().ToLowerInvariant();
            books = books.Where(b => (av == "available" && b.AvailableCopies > 0) || (av == "loaned" && b.AvailableCopies == 0)).ToList();
        }

        return books;
    }
}
