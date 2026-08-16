using MediatR;
using Domain.Service.DTOs.Loans;

namespace Domain.Service.Features.Loans.Queries.GetActiveLoans;

public class GetActiveLoansQuery : IRequest<LoanPageResponseDto>
{
    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
