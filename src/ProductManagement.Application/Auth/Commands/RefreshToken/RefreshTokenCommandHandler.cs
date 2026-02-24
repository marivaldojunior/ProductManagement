using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Busca usuário que possui este refresh token
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null)
            return Result.Failure<AuthResponse>("Invalid refresh token.");

        // Verifica se conta está ativa
        if (!user.IsActive)
            return Result.Failure<AuthResponse>("Account is deactivated.");

        // Obtém o refresh token ativo
        var existingRefreshToken = user.GetActiveRefreshToken(request.RefreshToken);

        if (existingRefreshToken is null)
            return Result.Failure<AuthResponse>("Refresh token is expired or revoked.");

        // Rotação de Refresh Token: revoga o token atual e gera um novo
        // Isso mitiga riscos caso o token seja comprometido
        user.RevokeRefreshToken(request.RefreshToken);

        // Gera novo Access Token
        var accessToken = _tokenService.GenerateAccessToken(user);

        // Gera novo Refresh Token (rotation)
        var newRefreshToken = user.AddRefreshToken(expirationDays: 7);

        // Persiste
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            user.Id,
            user.Email.Value,
            user.FullName,
            accessToken,
            newRefreshToken.Token,
            DateTime.UtcNow.AddMinutes(15));
    }
}
