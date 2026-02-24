using MediatR;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;

namespace ProductManagement.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
