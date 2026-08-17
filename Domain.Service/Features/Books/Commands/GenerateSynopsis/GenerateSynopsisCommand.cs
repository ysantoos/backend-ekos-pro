using MediatR;
using Domain.Service.DTOs;

namespace Domain.Service.Features.Books.Commands.GenerateSynopsis;

public class GenerateSynopsisCommand : IRequest<string>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}
