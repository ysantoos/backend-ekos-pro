using Domain.Service.DTOs;
using Domain.Service.DTOs.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend_ekos_pro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IMediator mediator, ILogger<CatalogController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? category, [FromQuery] string? author, [FromQuery] int? publicationYear, [FromQuery] string? availability, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Get catalog called with search={search} category={category} author={author} publicationYear={year} availability={availability}", search, category, author, publicationYear, availability);

        var query = new Domain.Service.Features.Catalog.Queries.GetCatalogBooksQuery
        {
            Search = search,
            Category = category,
            Author = author,
            PublicationYear = publicationYear,
            Availability = availability,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        var response = ApiResponse<CatalogPageResponseDto>.SuccessResponse(result);
        return Ok(response);
    }
}
