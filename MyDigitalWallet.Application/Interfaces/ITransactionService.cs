using MyDigitalWallet.Domain.Entities;

namespace MyDigitalWallet.Application.Interfaces;

public interface ITransactionService
{
    Task<Guid> TransferAsync(Guid fromUserId, Guid toUserId, decimal amount);
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, DateTime? startDate, DateTime? endDate);
}
