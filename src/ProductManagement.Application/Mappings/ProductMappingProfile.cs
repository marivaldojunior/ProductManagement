using AutoMapper;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForCtorParam("Price", opt => opt.MapFrom(src => src.Price.Amount))
            .ForCtorParam("Currency", opt => opt.MapFrom(src => src.Price.Currency));
    }
}
