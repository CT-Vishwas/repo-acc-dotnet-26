
using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController: ControllerBase
{
    private readonly IProductService _service;

    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _service = productService;
        _logger = logger;
    }
    
    [Authorize(Roles ="Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO productRequestDTO)
    {
        _logger.LogInformation("Creating a Product: {productRequestDTO.Name}", productRequestDTO.Name);
        var product =  await _service.CreateProduct(productRequestDTO);
        if (product == null) return NotFound();

        return CreatedAtAction(nameof(GetById), new { id = product.Id}, new ApiResponse<ProductResponseDTO>(true, "Product Created successfully", product, []));
    }

    [HttpGet]
    [ResponseCache(Duration = 60, Location =ResponseCacheLocation.Client)]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequestDTO productRequest)
    {
        var product = await _service.UpdateProduct(id, productRequest);
        
        return Ok(new ApiResponse<ProductResponseDTO>(true, "Product updated successfully", product, []));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _service.DeleteProduct(id);
        return Ok(new ApiResponse<ProductResponseDTO?>(true, "Product deleted successfully", null, []));
    }

    [HttpGet("pages")]
    public async Task<IActionResult> GetAll([FromQuery] ProductParameters parameters)
    {
        var response = await _service.GetPagedResponseAsync(parameters);
        return Ok(response);
    }
}