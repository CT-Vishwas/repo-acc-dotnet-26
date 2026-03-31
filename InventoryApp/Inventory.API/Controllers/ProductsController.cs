
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
    public Task<IActionResult> CreateProduct()
    {
        
    }
}