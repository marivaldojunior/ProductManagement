using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Auth.Commands.LoginUser;
using ProductManagement.Application.Auth.Commands.RefreshToken;
using ProductManagement.Application.Auth.Commands.RegisterUser;
using ProductManagement.Application.DTOs.Auth;

namespace ProductManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Password and confirmation do not match.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            // Email já existe
            if (result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = result.Error,
                    Status = StatusCodes.Status409Conflict
                });
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(Register), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Autentica um usuário e retorna tokens JWT
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication Failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Renova os tokens usando um Refresh Token válido
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Token Refresh Failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result.Value);
    }
}
