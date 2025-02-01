using Microsoft.AspNetCore.Mvc;
using MyDigitalWallet.Application.Interfaces;

namespace MyDigitalWallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var userId = await _userService.CreateUserAsync(request.Username, request.Password, request.Email);
            return Ok(new { UserId = userId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (token == null)
            return Unauthorized("Usuário ou senha inválidos.");

        return Ok(new { Token = token });
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class LoginRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
