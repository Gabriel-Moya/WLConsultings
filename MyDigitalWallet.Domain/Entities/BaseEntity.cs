namespace MyDigitalWallet.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}
