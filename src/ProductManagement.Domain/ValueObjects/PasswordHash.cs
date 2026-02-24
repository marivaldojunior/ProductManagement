using ProductManagement.Domain.Common;

namespace ProductManagement.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o hash da senha (nunca armazena a senha em texto plano)
/// </summary>
public sealed class PasswordHash : IEquatable<PasswordHash>
{
    public string Value { get; }

    private PasswordHash(string hashedValue) => Value = hashedValue;

    /// <summary>
    /// Cria um PasswordHash a partir de um hash já calculado (usado pelo repositório)
    /// </summary>
    public static PasswordHash FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty.", nameof(hash));

        return new PasswordHash(hash);
    }

    /// <summary>
    /// Valida requisitos mínimos de senha antes do hashing
    /// </summary>
    public static Result ValidatePasswordStrength(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return Result.Failure("Password cannot be empty.");

        if (plainPassword.Length < 8)
            return Result.Failure("Password must be at least 8 characters long.");

        if (plainPassword.Length > 128)
            return Result.Failure("Password is too long.");

        if (!plainPassword.Any(char.IsUpper))
            return Result.Failure("Password must contain at least one uppercase letter.");

        if (!plainPassword.Any(char.IsLower))
            return Result.Failure("Password must contain at least one lowercase letter.");

        if (!plainPassword.Any(char.IsDigit))
            return Result.Failure("Password must contain at least one digit.");

        if (!plainPassword.Any(c => !char.IsLetterOrDigit(c)))
            return Result.Failure("Password must contain at least one special character.");

        return Result.Success();
    }

    public bool Equals(PasswordHash? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as PasswordHash);
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(PasswordHash hash) => hash.Value;
}
