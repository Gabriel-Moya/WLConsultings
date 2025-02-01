using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDigitalWallet.Application.Interfaces;
using System.Security.Claims;

namespace MyDigitalWallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var fromUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);

        try
        {
            var transactionId = await _transactionService.TransferAsync(
                fromUserId,
                request.ToUserId,
                request.Amount
            );

            return Ok(new { TransactionId = transactionId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var startUtc = startDate?.ToUniversalTime();
        var endUtc = endDate?.ToUniversalTime();

        try
        {
            var transactions = await _transactionService.GetUserTransactionsAsync(userId, startUtc, endUtc);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class TransferRequest
{
    public Guid ToUserId { get; set; }
    public decimal Amount { get; set; }
}
