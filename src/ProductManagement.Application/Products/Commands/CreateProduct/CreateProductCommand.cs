using MediatR;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string? Sku) : IRequest<ProductDto>;
