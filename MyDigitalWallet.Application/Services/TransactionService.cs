using MyDigitalWallet.Application.Interfaces;
using MyDigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyDigitalWallet.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IGenericRepository<Wallet> _walletRepository;

    public TransactionService(
        IGenericRepository<Transaction> transactionRepository,
        IGenericRepository<Wallet> walletRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<Guid> TransferAsync(Guid fromUserId, Guid toUserId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("Valor de transferência inválido.");

        var fromWallet = await _walletRepository.Query().FirstOrDefaultAsync(w => w.UserId == fromUserId);
        var toWallet = await _walletRepository.Query().FirstOrDefaultAsync(w => w.UserId == toUserId);

        if (fromWallet == null || toWallet == null)
            throw new Exception("Carteira de origem ou destino não encontrada.");

        if (fromWallet.Balance < amount)
            throw new Exception("Saldo insuficiente.");

        fromWallet.Balance -= amount;
        toWallet.Balance += amount;

        await _walletRepository.UpdateAsync(fromWallet);
        await _walletRepository.UpdateAsync(toWallet);

        var transaction = new Transaction
        {
            FromWalletId = fromWallet.Id,
            ToWalletId = toWallet.Id,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        return transaction.Id;
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(
        Guid userId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var wallet = await _walletRepository
            .Query()
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new Exception("Carteira não encontrada.");

        var query = _transactionRepository.Query()
            .Where(t => t.FromWalletId == wallet.Id || t.ToWalletId == wallet.Id);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
