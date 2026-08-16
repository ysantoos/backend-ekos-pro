using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Domain.Service.Features.Catalog.Commands.DeleteCatalogBook;
using Infrastructure.Entity.Data;

namespace Infrastructure.Entity.Features.Catalog.Commands.DeleteCatalogBook;

public class DeleteCatalogBookHandler : IRequestHandler<DeleteCatalogBookCommand, Unit>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<DeleteCatalogBookHandler> _logger;

    public DeleteCatalogBookHandler(ApplicationDbContext db, ILogger<DeleteCatalogBookHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCatalogBookCommand request, CancellationToken cancellationToken)
    {
        // Ensure entity is tracked (DbContext configured with NoTracking by default)
        var book = await _db.CatalogBooks
            .AsTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
            throw new NotFoundException("CatalogBook", request.Id);

        // Check for active loans
        var hasActiveLoans = await _db.LoanHistoryEntries
            .AnyAsync(l => l.BookId == book.Id.ToString() && !l.IsReturned, cancellationToken);

        if (hasActiveLoans)
            throw new BusinessException("Cannot delete book: there are active loans for this book.");

        // Soft-delete
        book.IsDeleted = true;
        book.DeletedAt = DateTime.UtcNow;
        // DeletedBy should be set from user context; left null for now

        var saved = await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("DeleteCatalogBookHandler: SaveChanges returned {Saved} for book {BookId}", saved, request.Id);

        return Unit.Value;
    }
}
