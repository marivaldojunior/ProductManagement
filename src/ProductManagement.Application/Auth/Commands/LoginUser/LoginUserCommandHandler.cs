using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        // Busca usuário por email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Resposta genérica para evitar enumeração de usuários
        if (user is null)
            return Result.Failure<AuthResponse>("Invalid email or password.");

        // Verifica se conta está ativa
        if (!user.IsActive)
            return Result.Failure<AuthResponse>("Account is deactivated. Please contact support.");

        // Verifica senha
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash.Value))
            return Result.Failure<AuthResponse>("Invalid email or password.");

        // Gera Access Token JWT
        var accessToken = _tokenService.GenerateAccessToken(user);

        // Gera Refresh Token e associa ao usuário
        var refreshToken = user.AddRefreshToken(expirationDays: 7);

        // Registra login
        user.RecordLogin();

        // Persiste alterações
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            user.Id,
            user.Email.Value,
            user.FullName,
            accessToken,
            refreshToken.Token,
            DateTime.UtcNow.AddMinutes(15)); // Access token expira em 15 min
    }
}
