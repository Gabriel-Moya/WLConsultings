using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDigitalWallet.Application.Interfaces;
using System.Security.Claims;

namespace MyDigitalWallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IUserService _userService;

    public WalletController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var balance = await _userService.GetBalanceAsync(userId);
        return Ok(new { Balance = balance });
    }

    [HttpPost("add-balance")]
    public async Task<IActionResult> AddBalance([FromBody] AddBalanceRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        try
        {
            await _userService.AddBalanceAsync(userId, request.Amount);
            return Ok("Saldo adicionado com sucesso.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class AddBalanceRequest
{
    public decimal Amount { get; set; }
}
