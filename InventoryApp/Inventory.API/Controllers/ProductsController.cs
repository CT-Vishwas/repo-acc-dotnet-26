
using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController: ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService productService)
    {
        _service = productService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO productRequestDTO)
    {
        var product =  await _service.CreateProduct(productRequestDTO);
        if (product == null) return NotFound();

        return CreatedAtAction(nameof(GetById), new { id = product.Id}, new ApiResponse<ProductResponseDTO>(true, "Product Created successfully", product, []));
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products =  await _service.GetAllProducts();
        if (products == null) return NotFound();

        return Ok(new ApiResponse<IEnumerable<ProductResponseDTO>>(true, "Products fetched successfully", products, []));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product =  await _service.GetById(id);
        if (product == null) return NotFound();

        return Ok(new ApiResponse<ProductResponseDTO>(true, "Product fetched successfully", product, []));
    }

}