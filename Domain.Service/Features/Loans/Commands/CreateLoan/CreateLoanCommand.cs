using MediatR;
using Domain.Service.DTOs.Loans;

namespace Domain.Service.Features.Loans.Commands.CreateLoan;

public class CreateLoanCommand : IRequest<LoanDetailDto>
{
    public string BookId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? MobilePhone { get; set; }

    public string? Email { get; set; }

    public DateTime? ReturnDate { get; set; }

    // The following fields are ignored on create and enforced by the server:
    // LoanDate, IsDeleted, DeletedBy, DeletedAt, IsReturned
}
