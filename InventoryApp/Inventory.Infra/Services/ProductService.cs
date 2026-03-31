using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;

namespace Inventory.Infra.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productRequestDTO)
    {
        
    }
}