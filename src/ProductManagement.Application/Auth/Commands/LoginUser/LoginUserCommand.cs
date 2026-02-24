using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;

namespace ProductManagement.Application.Auth.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
