using MediatR;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
