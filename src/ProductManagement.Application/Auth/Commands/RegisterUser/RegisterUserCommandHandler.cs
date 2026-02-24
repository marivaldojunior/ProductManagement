using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Valida força da senha
        var passwordValidation = PasswordHash.ValidatePasswordStrength(request.Password);
        if (passwordValidation.IsFailure)
            return Result.Failure<UserResponse>(passwordValidation.Error);

        // Verifica se email já existe
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure<UserResponse>("An account with this email already exists.");

        // Gera hash seguro da senha
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // Cria a entidade User
        var userResult = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword);

        if (userResult.IsFailure)
            return Result.Failure<UserResponse>(userResult.Error);

        var user = userResult.Value;

        // Persiste
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.IsActive,
            user.CreatedAt);
    }
}
