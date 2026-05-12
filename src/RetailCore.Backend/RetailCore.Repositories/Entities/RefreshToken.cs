namespace RetailCore.Repositories.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsActive => !IsRevoked && !IsExpired;
}
