using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Users;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginResultDto>;

public record LoginResultDto(string Token, Guid UserId, string FullName, string Role);

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginUserHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedException("Credenciales inválidas.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash!))
            throw new UnauthorizedException("Credenciales inválidas.");

        var token = _jwtService.GenerateToken(user);

        return new LoginResultDto(token, user.Id, user.FullName, user.Role.ToString());
    }
}