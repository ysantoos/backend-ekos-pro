using MediatR;
using Domain.Service.DTOs.Loans;

namespace Domain.Service.Features.Loans.Queries.GetLoanHistoryByBook;

public class GetLoanHistoryByBookQuery : IRequest<IEnumerable<LoanDto>>
{
    public Guid BookId { get; set; }
}
