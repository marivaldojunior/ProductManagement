using MediatR;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
