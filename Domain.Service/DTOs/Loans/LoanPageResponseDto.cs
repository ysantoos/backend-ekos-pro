using System.Collections.Generic;

namespace Domain.Service.DTOs.Loans;

public class LoanPageResponseDto
{
    public IEnumerable<LoanDto> Items { get; set; } = Array.Empty<LoanDto>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public long TotalCount { get; set; }

    public bool HasMore { get; set; }
}
