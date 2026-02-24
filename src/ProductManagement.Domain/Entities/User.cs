using ProductManagement.Domain.Common;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Domain.Entities;

/// <summary>
/// Entidade User rica em comportamento - sem setters públicos anêmicos
/// </summary>
public class User : Entity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { } // EF Core

    private User(string firstName, string lastName, Email email, PasswordHash passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public static Result<User> Create(
        string firstName,
        string lastName,
        string email,
        string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<User>("First name is required.");

        if (firstName.Length > 100)
            return Result.Failure<User>("First name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<User>("Last name is required.");

        if (lastName.Length > 100)
            return Result.Failure<User>("Last name cannot exceed 100 characters.");

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        var passwordHashVO = PasswordHash.FromHash(hashedPassword);

        return new User(
            firstName.Trim(),
            lastName.Trim(),
            emailResult.Value,
            passwordHashVO);
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (!string.IsNullOrWhiteSpace(firstName) && firstName.Length <= 100)
            FirstName = firstName.Trim();

        if (!string.IsNullOrWhiteSpace(lastName) && lastName.Length <= 100)
            LastName = lastName.Trim();

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newHashedPassword)
    {
        PasswordHash = PasswordHash.FromHash(newHashedPassword);
        UpdatedAt = DateTime.UtcNow;

        // Revoga todos os refresh tokens por segurança
        RevokeAllRefreshTokens();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        RevokeAllRefreshTokens();
    }

    public RefreshToken AddRefreshToken(int expirationDays = 7)
    {
        // Remove tokens antigos expirados ou revogados (manter histórico limpo)
        _refreshTokens.RemoveAll(t => !t.IsActive);

        var refreshToken = RefreshToken.Create(expirationDays);
        _refreshTokens.Add(refreshToken);

        return refreshToken;
    }

    public RefreshToken? GetActiveRefreshToken(string token)
    {
        return _refreshTokens.FirstOrDefault(t => t.Token == token && t.IsActive);
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        refreshToken?.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }
    }

    public string FullName => $"{FirstName} {LastName}";
}
