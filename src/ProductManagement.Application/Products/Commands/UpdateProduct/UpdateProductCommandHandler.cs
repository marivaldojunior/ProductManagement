using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Exceptions;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        if (!string.IsNullOrWhiteSpace(request.Sku))
        {
            var existingProduct = await _unitOfWork.Products.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct is not null && existingProduct.Id != request.Id)
            {
                throw new DomainException($"A product with SKU '{request.Sku}' already exists.");
            }
        }

        var price = new Money(request.Price, request.Currency);

        product.Update(
            request.Name,
            request.Description,
            price,
            request.StockQuantity,
            request.Sku);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
