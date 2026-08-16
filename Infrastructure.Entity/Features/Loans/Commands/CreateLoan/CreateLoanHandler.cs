using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.DTOs.Loans;
using Domain.Service.Exceptions;
using Domain.Service.Features.Loans.Commands.CreateLoan;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Loans.Commands.CreateLoan;

public class CreateLoanHandler : IRequestHandler<CreateLoanCommand, LoanDetailDto>
{
    private readonly ApplicationDbContext _db;

    public CreateLoanHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LoanDetailDto> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        // Validate return date
        var now = DateTime.UtcNow.Date;
        if (!request.ReturnDate.HasValue || request.ReturnDate.Value.Date <= now)
            throw new ValidationException("ReturnDate must be a future date.");

        // Find book
        var book = await _db.CatalogBooks.FirstOrDefaultAsync(b => b.Id.ToString() == request.BookId, cancellationToken);
        if (book == null)
            throw new NotFoundException("CatalogBook", request.BookId);

        // Calculate available copies
        var loanedCount = await _db.LoanHistoryEntries
            .AsNoTracking()
            .Where(l => !l.IsReturned && l.BookId == request.BookId)
            .CountAsync(cancellationToken);

        var available = Math.Max(0, book.TotalCopies - loanedCount);
        if (available <= 0)
            throw new BusinessException("No available copies for this book.");

        // Create loan entry
        var entity = new Domain.Service.Entities.LoanHistoryEntry
        {
            BookId = request.BookId,
            FullName = request.FullName,
            Email = request.Email,
            MobilePhone = request.MobilePhone,
            LoanDate = DateTime.UtcNow, // server enforces current date/time
            ReturnDate = request.ReturnDate,
            IsReturned = false,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

        _db.LoanHistoryEntries.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var duration = (entity.ReturnDate.HasValue) ? (int)(entity.ReturnDate.Value.Date - entity.LoanDate.Value.Date).TotalDays : 0;

        return new LoanDetailDto
        {
            Id = entity.Id,
            BookId = entity.BookId,
            FullName = entity.FullName,
            MobilePhone = entity.MobilePhone,
            Email = entity.Email,
            LoanDate = entity.LoanDate ?? DateTime.UtcNow,
            ReturnDate = entity.ReturnDate,
            DurationDays = duration,
            IsReturned = entity.IsReturned,
            IsDeleted = entity.IsDeleted,
            DeletedBy = entity.DeletedBy,
            DeletedAt = entity.DeletedAt
        };
    }
}
