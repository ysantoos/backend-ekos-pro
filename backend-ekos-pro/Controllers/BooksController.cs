using Domain.Service.DTOs;
using Domain.Service.DTOs.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend_ekos_pro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IMediator mediator, ILogger<BooksController> logger)
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

    [HttpGet("{bookId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid bookId)
    {
        _logger.LogInformation("Get book by id called: {bookId}", bookId);

        try
        {
            var query = new Domain.Service.Features.Catalog.Queries.GetCatalogBookById.GetCatalogBookByIdQuery { Id = bookId };
            var book = await _mediator.Send(query);
            return Ok(ApiResponse<CatalogBookDto>.SuccessResponse(book, "Book found."));
        }
        catch (Domain.Service.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning(ex, "Book not found: {bookId}", bookId);
            return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        _logger.LogInformation("Delete catalog book called with id={id}", id);

        try
        {
            var cmd = new Domain.Service.Features.Catalog.Commands.DeleteCatalogBook.DeleteCatalogBookCommand { Id = id };
            await _mediator.Send(cmd);
            var resp = ApiResponse<object>.SuccessResponse(null, "Book deleted successfully");
            return Ok(resp);
        }
        catch (Domain.Service.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning(ex, "Book not found: {id}", id);
            return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Domain.Service.Exceptions.BusinessException ex)
        {
            _logger.LogWarning(ex, "Business rule prevented deletion for book {id}", id);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Service.Features.Catalog.Commands.CreateCatalogBook.CreateCatalogBookCommand command)
    {
        _logger.LogInformation("Create catalog book called with isbn={isbn}", command.Isbn);

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { }, ApiResponse< CatalogBookDto>.SuccessResponse(result, "Book created successfully"));
        }
        catch (Domain.Service.Exceptions.BusinessException ex)
        {
            _logger.LogWarning(ex, "Business rule prevented creation for isbn={isbn}", command.Isbn);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] Domain.Service.Features.Catalog.Commands.UpdateCatalogBook.UpdateCatalogBookCommand command)
    {
        _logger.LogInformation("Update catalog book called id={id} isbn={isbn}", id, command.Isbn);

        if (id != command.Id)
            return BadRequest(ApiResponse<object>.FailureResponse("Id in route does not match id in body"));

        try
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<CatalogBookDto>.SuccessResponse(result, "Book updated successfully"));
        }
        catch (Domain.Service.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning(ex, "Book not found: {id}", id);
            return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Domain.Service.Exceptions.BusinessException ex)
        {
            _logger.LogWarning(ex, "Business rule prevented update for book {id}", id);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }
}
