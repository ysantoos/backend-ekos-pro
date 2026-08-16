using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Loans;
using Infrastructure.Entity.Data;
using Domain.Service.Features.Loans.Queries.GetLoanHistoryByBook;

namespace Infrastructure.Entity.Features.Loans.Queries.GetLoanHistoryByBook;

public class GetLoanHistoryByBookHandler : IRequestHandler<GetLoanHistoryByBookQuery, IEnumerable<LoanDto>>
{
    private readonly ApplicationDbContext _db;

    public GetLoanHistoryByBookHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<LoanDto>> Handle(GetLoanHistoryByBookQuery request, CancellationToken cancellationToken)
    {
        var bookIdStr = request.BookId.ToString();

        var items = await _db.LoanHistoryEntries
            .AsNoTracking()
            .Where(l => l.BookId == bookIdStr)
            .OrderByDescending(l => l.LoanDate)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                UserName = l.FullName,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate
            })
            .ToListAsync(cancellationToken);

        return items;
    }
}
