

using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace  Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;


    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDTO request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e=> e.Description).ToList();
            return BadRequest(new ApiResponse<object?>(false, "Registration Failed", null, errors));
        }

        // var response = new UserResponseDTO(request.Username, request.Role);

        return Ok(new ApiResponse<object?>(true, "Registration Sucessful", request.Username, []));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        if(result == null)
        {
            return Unauthorized(new ApiResponse<UserResponseDTO?>(false, "Invalid Credentials", null,[]));
        }
        return Ok(new ApiResponse<UserResponseDTO?>(true, "Login Successful", result,[]));
    }

}