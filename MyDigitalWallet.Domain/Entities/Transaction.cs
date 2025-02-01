namespace MyDigitalWallet.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid FromWalletId { get; set; }
    public Guid ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Wallet FromWallet { get; set; } = default!;
    public Wallet ToWallet { get; set; } = default!;
}
