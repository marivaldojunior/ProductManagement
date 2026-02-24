using ProductManagement.Domain.Entities;

namespace ProductManagement.Domain.Interfaces;

/// <summary>
/// Contrato para serviço de geração/validação de tokens JWT
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um Access Token JWT para o usuário
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Valida um Access Token e retorna as claims se válido
    /// </summary>
    AccessTokenValidationResult ValidateAccessToken(string token);
}

public record AccessTokenValidationResult(bool IsValid, Guid? UserId, string? Error);
