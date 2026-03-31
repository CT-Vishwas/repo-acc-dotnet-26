using AutoMapper;
using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;

namespace Inventory.Infra.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productRequestDTO)
    {
        var product = await _productRepository.AddAsync(_mapper.Map<Product>(productRequestDTO));

        return _mapper.Map<ProductResponseDTO>(product);
    }
}