

using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;

namespace Inventory.Core.Interfaces;

public interface IProductService
{
    Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productRequestDTO);
    Task<IEnumerable<ProductResponseDTO>> GetAllProducts();

    Task<ProductResponseDTO> GetById(int id);
}