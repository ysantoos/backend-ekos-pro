using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Loans;
using Infrastructure.Entity.Data;
using Domain.Service.Features.Loans.Queries.GetActiveLoans;

namespace Infrastructure.Entity.Features.Loans.Queries.GetActiveLoans;

public class GetActiveLoansHandler : IRequestHandler<GetActiveLoansQuery, LoanPageResponseDto>
{
    private readonly ApplicationDbContext _db;

    public GetActiveLoansHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LoanPageResponseDto> Handle(GetActiveLoansQuery request, CancellationToken cancellationToken)
    {
        var query = _db.LoanHistoryEntries.AsNoTracking().Where(l => !l.IsReturned);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();

            // Search by user full name or by book title (book title lives in CatalogBooks)
            // First find matching book ids by title, then filter loans whose BookId is in that set
            var matchingBookIds = await _db.CatalogBooks
                .AsNoTracking()
                .Where(b => b.Title.Contains(s))
                .Select(b => b.Id.ToString())
                .ToListAsync(cancellationToken);

            query = query.Where(l => l.FullName.Contains(s) || matchingBookIds.Contains(l.BookId));
        }

        var total = await query.CountAsync(cancellationToken);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Max(1, request.PageSize);
        var skip = (page - 1) * pageSize;

        var items = await query
            .OrderByDescending(l => l.LoanDate)
            .Skip(skip)
            .Take(pageSize)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                FullName = l.FullName,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate,
                IsReturned = l.IsReturned
            })
            .ToListAsync(cancellationToken);

        // Enrich with book title/author where possible
        // Parse stored BookId strings to GUIDs and query CatalogBooks by Guid to avoid string-formatting mismatches.
        // Also ignore global query filters so we can show title/author even if the book was soft-deleted.
        var bookIds = items
            .Select(i => i.BookId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();

        var books = new Dictionary<Guid, (string Title, string Author)>();
        if (bookIds.Count > 0)
        {
            books = await _db.CatalogBooks
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(b => bookIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, b => (b.Title, b.Author), cancellationToken);
        }

        foreach (var it in items)
        {
            if (Guid.TryParse(it.BookId, out var gid) && books.TryGetValue(gid, out var info))
            {
                it.BookTitle = info.Title;
                it.BookAuthor = info.Author;
            }
        }

        return new LoanPageResponseDto
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            HasMore = (skip + items.Count) < total
        };
    }
}
