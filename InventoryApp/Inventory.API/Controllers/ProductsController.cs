
using Inventory.Core.DTOs.Requests;
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

        return Ok(product);
    }
}