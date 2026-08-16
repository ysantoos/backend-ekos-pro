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
            // LoanHistoryEntry stores user full name in FullName
            query = query.Where(l => l.FullName.Contains(s) || l.BookId.Contains(s));
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
                ReturnDate = l.ReturnDate
            })
            .ToListAsync(cancellationToken);

        // Enrich with book title/author where possible
        var bookIds = items.Select(i => i.BookId).Distinct().ToList();
        var books = await _db.CatalogBooks
            .AsNoTracking()
            .Where(b => bookIds.Contains(b.Id.ToString()))
            .ToDictionaryAsync(b => b.Id.ToString(), b => new { b.Title, b.Author }, cancellationToken);

        foreach (var it in items)
        {
            if (books.TryGetValue(it.BookId, out var info))
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
