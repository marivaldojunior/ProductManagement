using System.Text.RegularExpressions;
using ProductManagement.Domain.Common;

namespace ProductManagement.Domain.ValueObjects;

public sealed partial class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email cannot be empty.");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 256)
            return Result.Failure<Email>("Email is too long.");

        if (!EmailRegex().IsMatch(email))
            return Result.Failure<Email>("Email format is invalid.");

        return new Email(email);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as Email);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
