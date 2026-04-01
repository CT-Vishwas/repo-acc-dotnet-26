

using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;

namespace Inventory.Core.Interfaces;

public interface IProductService
{
    Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productRequestDTO);
    Task DeleteProduct(int id);
    Task<IEnumerable<ProductResponseDTO>> GetAllProducts();

    Task<ProductResponseDTO> GetById(int id);
    Task<ProductResponseDTO> UpdateProduct(int id, ProductRequestDTO productRequest);
}