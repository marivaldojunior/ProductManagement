using System.Security.Cryptography;

namespace ProductManagement.Domain.ValueObjects;

/// <summary>
/// Value Object que encapsula um Refresh Token seguro
/// </summary>
public sealed class RefreshToken
{
    public string Token { get; }
    public DateTime ExpiresAt { get; }
    public DateTime CreatedAt { get; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken(string token, DateTime expiresAt, DateTime createdAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        IsRevoked = false;
    }

    /// <summary>
    /// Gera um novo Refresh Token criptograficamente seguro
    /// </summary>
    public static RefreshToken Create(int expirationDays = 7)
    {
        // Gera 64 bytes aleatórios usando um gerador criptograficamente seguro
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken(
            token,
            DateTime.UtcNow.AddDays(expirationDays),
            DateTime.UtcNow);
    }

    /// <summary>
    /// Reconstrói um RefreshToken existente (usado pelo repositório)
    /// </summary>
    public static RefreshToken FromExisting(
        string token,
        DateTime expiresAt,
        DateTime createdAt,
        bool isRevoked,
        DateTime? revokedAt)
    {
        var refreshToken = new RefreshToken(token, expiresAt, createdAt)
        {
            IsRevoked = isRevoked,
            RevokedAt = revokedAt
        };
        return refreshToken;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        if (!IsRevoked)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }
    }
}
