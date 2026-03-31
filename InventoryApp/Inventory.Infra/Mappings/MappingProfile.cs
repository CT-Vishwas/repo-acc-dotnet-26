
using AutoMapper;
using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Entities;

namespace Inventory.Infra.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        // Entity -> DTO
        CreateMap<Product, ProductResponseDTO>()
        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

        //DTO -> Entity
        CreateMap<ProductRequestDTO, Product>()
        .ForMember(dest=> dest.Category, opt=>opt.MapFrom(src=> Enum.Parse<ProductCategory>(src.Category, true)));
    }
}