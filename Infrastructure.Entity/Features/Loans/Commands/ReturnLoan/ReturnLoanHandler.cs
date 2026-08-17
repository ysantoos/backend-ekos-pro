using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Domain.Service.Features.Loans.Commands.ReturnLoan;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Loans.Commands.ReturnLoan;

public class ReturnLoanHandler : IRequestHandler<ReturnLoanCommand, Unit>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ReturnLoanHandler> _logger;

    public ReturnLoanHandler(ApplicationDbContext db, ILogger<ReturnLoanHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Unit> Handle(ReturnLoanCommand request, CancellationToken cancellationToken)
    {
        // Ensure the entity is tracked so SaveChanges can persist the update
        var loan = await _db.LoanHistoryEntries
            .AsTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (loan == null)
            throw new NotFoundException("LoanHistoryEntry", request.Id);

        if (loan.IsReturned)
        {
            // Already returned
            throw new BusinessException("The book has already been returned.");
        }

        loan.IsReturned = true;
        // Optionally set ReturnDate to now if not present
        if (!loan.ReturnDate.HasValue)
            loan.ReturnDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ReturnLoanHandler: Marked loan {LoanId} as returned.", request.Id);
        return Unit.Value;
    }
}
