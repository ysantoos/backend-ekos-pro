using Domain.Service.DTOs;
using Domain.Service.DTOs.Loans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend_ekos_pro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoansController> _logger;

    public LoansController(IMediator mediator, ILogger<LoansController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Get active loans called with search={search} page={page} pageSize={pageSize}", search, page, pageSize);

        var query = new Domain.Service.Features.Loans.Queries.GetActiveLoans.GetActiveLoansQuery
        {
            Search = search,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(ApiResponse<LoanPageResponseDto>.SuccessResponse(result, "Active loans loaded."));
    }

    [HttpGet("{bookId:guid}/loan-history")]
    public async Task<IActionResult> GetHistory([FromRoute] Guid bookId)
    {
        _logger.LogInformation("Get loan history called for bookId={bookId}", bookId);

        var query = new Domain.Service.Features.Loans.Queries.GetLoanHistoryByBook.GetLoanHistoryByBookQuery { BookId = bookId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(result, "Loan history loaded."));
    }
}
