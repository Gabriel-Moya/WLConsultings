namespace MyDigitalWallet.Application.Interfaces;

public interface IUserService
{
    Task<Guid> CreateUserAsync(string username, string password, string email);
    Task<string?> AuthenticateAsync(string username, string password);
    Task<decimal> GetBalanceAsync(Guid userId);
    Task AddBalanceAsync(Guid userId, decimal amount);
}
