using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Exceptions;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Sku))
        {
            var existingProduct = await _unitOfWork.Products.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct is not null)
            {
                throw new DomainException($"A product with SKU '{request.Sku}' already exists.");
            }
        }

        var price = new Money(request.Price, request.Currency);

        var product = new Product(
            request.Name,
            request.Description,
            price,
            request.StockQuantity,
            request.Sku);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
