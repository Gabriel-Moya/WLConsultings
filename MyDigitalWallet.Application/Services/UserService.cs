using MyDigitalWallet.Application.Interfaces;
using MyDigitalWallet.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MyDigitalWallet.Application.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Wallet> _walletRepository;
    private readonly IJwtService _jwtService;

    public UserService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Wallet> walletRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _jwtService = jwtService;
    }

    public async Task<Guid> CreateUserAsync(string username, string password, string email)
    {
        var existingUser = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

        if (existingUser != null)
            throw new Exception("Usuário ou e-mail já existente.");

        var passwordHash = HashPassword(password);

        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Email = email
        };

        await _userRepository.AddAsync(newUser);

        var newWallet = new Wallet
        {
            UserId = newUser.Id,
            Balance = 0
        };

        await _walletRepository.AddAsync(newWallet);
        await _userRepository.SaveChangesAsync();

        return newUser.Id;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user.Id, user.Username);
        return token;
    }

    public async Task<decimal> GetBalanceAsync(Guid userId)
    {
        var userWallet = await _walletRepository
            .Query()
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (userWallet == null)
            throw new Exception("Carteira não encontrada.");

        return userWallet.Balance;
    }

    public async Task AddBalanceAsync(Guid userId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("Valor inválido para depósito.");

        var wallet = await _walletRepository
            .Query()
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
            throw new Exception("Carteira não encontrada.");

        wallet.Balance += amount;
        await _walletRepository.UpdateAsync(wallet);
        await _walletRepository.SaveChangesAsync();
    }

    // Métodos internos para hash de senha
    private string HashPassword(string password)
    {
        // Exemplo simples de hash com salt
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Armazenar salt e hash juntos (forma simples)
        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        string hashed = parts[1];

        string reHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashed == reHashed;
    }
}
