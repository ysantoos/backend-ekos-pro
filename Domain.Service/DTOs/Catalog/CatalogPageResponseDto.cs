using System.Collections.Generic;

namespace Domain.Service.DTOs.Catalog;

public class CatalogPageResponseDto
{
    public IEnumerable<CatalogBookDto> Items { get; set; } = Array.Empty<CatalogBookDto>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public long TotalCount { get; set; }

    public bool HasMore { get; set; }
}
