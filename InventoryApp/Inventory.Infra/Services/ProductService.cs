using System.Formats.Asn1;
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

    public async Task DeleteProduct(int id)
    {
        await _productRepository.DeleteAsync(id);
        return;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDTO>>(products);

        return productDtos;
    }

    public async Task<ProductResponseDTO> GetById(int id)
    {
        var product = await _productRepository.GetAsync(id);
        if(product is null)
        {
            throw new KeyNotFoundException("Product not Found");
        }

        return _mapper.Map<ProductResponseDTO>(product);
    }

    public async Task<PagedResponse<IEnumerable<ProductResponseDTO>>> GetPagedResponseAsync(ProductParameters parameters)
    {
        // Convert String to Enum
        ProductCategory? categoryEnum = null;

        if(!string.IsNullOrEmpty(parameters.Category) && Enum.TryParse<ProductCategory>(parameters.Category, true, out var result))
        {
            categoryEnum = result;
        }

        // use the repo
        var (items, totalCount) = await _productRepository.GetPagedProductsAsync(
            parameters.SearchTerm,
            categoryEnum,
            parameters.SortBy,
            parameters.PageNumber,
            parameters.PageSize
        );

        // Mapping
        var data = _mapper.Map<IEnumerable<ProductResponseDTO>>(items);

        return new PagedResponse<IEnumerable<ProductResponseDTO>>(
            data,
            totalCount,
            parameters.PageNumber,
            parameters.PageSize
        );
    }

    public async Task<ProductResponseDTO> UpdateProduct(int id, ProductRequestDTO productRequest)
    {
        var product = await _productRepository.UpdateAsync(_mapper.Map<Product>(productRequest));

        return _mapper.Map<ProductResponseDTO>(product);
    }
}