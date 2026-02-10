using MediatR;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string? Sku) : IRequest<ProductDto>;
