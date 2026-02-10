using ProductManagement.Domain.Enums;

namespace ProductManagement.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    ProductStatus Status,
    string? Sku,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
