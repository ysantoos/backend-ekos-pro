using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Catalog;
using Domain.Service.Features.Catalog.Queries;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Handlers;

public class GetCatalogBooksHandler : IRequestHandler<GetCatalogBooksQuery, CatalogPageResponseDto>
{
    private readonly ApplicationDbContext _db;

    public GetCatalogBooksHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CatalogPageResponseDto> Handle(GetCatalogBooksQuery request, CancellationToken cancellationToken)
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

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Max(1, request.PageSize);
        var skip = (page - 1) * pageSize;

        var books = await query
            .OrderBy(b => b.Title)
            .Skip(skip)
            .Take(pageSize)
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
        {
            return new CatalogPageResponseDto
            {
                Items = Array.Empty<CatalogBookDto>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (skip + 0) < totalCount
            };
        }

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

        var response = new CatalogPageResponseDto
        {
            Items = books,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = (skip + books.Count) < totalCount
        };

        return response;
    }
}
