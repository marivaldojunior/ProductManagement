using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Infrastructure.Authentication;

/// <summary>
/// Implementação de hashing de senhas usando BCrypt
/// BCrypt inclui salt automático e é resistente a ataques de timing
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    // Work factor: 12 é um bom equilíbrio entre segurança e performance
    // Cada incremento dobra o tempo de computação
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
