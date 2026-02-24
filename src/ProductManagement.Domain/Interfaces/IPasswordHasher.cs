namespace ProductManagement.Domain.Interfaces;

/// <summary>
/// Contrato para serviço de hashing de senhas (BCrypt/Argon2)
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera hash seguro da senha
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifica se a senha corresponde ao hash armazenado
    /// </summary>
    bool Verify(string password, string hash);
}
