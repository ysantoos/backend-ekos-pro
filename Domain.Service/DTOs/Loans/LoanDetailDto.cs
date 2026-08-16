namespace Domain.Service.DTOs.Loans;

public class LoanDetailDto
{
    public Guid Id { get; set; }

    public string BookId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? MobilePhone { get; set; }

    public string? Email { get; set; }

    public DateTime LoanDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int DurationDays { get; set; }

    public bool IsReturned { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}
