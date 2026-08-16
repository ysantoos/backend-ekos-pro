namespace Domain.Service.DTOs.Loans;

public class LoanDto
{
    public Guid Id { get; set; }

    public string BookId { get; set; } = string.Empty;

    public string? BookTitle { get; set; }

    public string? BookAuthor { get; set; }

    public string UserName { get; set; } = string.Empty;

    public DateTime? LoanDate { get; set; }

    public DateTime? ReturnDate { get; set; }
}
