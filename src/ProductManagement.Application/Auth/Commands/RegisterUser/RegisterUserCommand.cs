using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;

namespace ProductManagement.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<Result<UserResponse>>;
