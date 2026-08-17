using System;
using MediatR;

namespace Domain.Service.Features.Loans.Commands.ReturnLoan;

public class ReturnLoanCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
