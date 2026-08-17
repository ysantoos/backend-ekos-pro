using MediatR;
using Domain.Service.Features.Books.Commands.GenerateSynopsis;
using Domain.Service.Interfaces;
using Domain.Service.Exceptions;

namespace Domain.Service.Features.Books.Handlers;

public class GenerateSynopsisHandler : IRequestHandler<GenerateSynopsisCommand, string>
{
    private readonly ITextGenerationService _textService;

    public GenerateSynopsisHandler(ITextGenerationService textService)
    {
        _textService = textService;
    }

    public async Task<string> Handle(GenerateSynopsisCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
            throw new ValidationException("Title and Author are required.");

        // Delegate to text generation service
        var synopsis = await _textService.GenerateSynopsisAsync(request.Title.Trim(), request.Author.Trim(), cancellationToken);

        if (string.IsNullOrWhiteSpace(synopsis))
            throw new BusinessException("Failed to generate synopsis.");

        return synopsis.Trim();
    }
}
