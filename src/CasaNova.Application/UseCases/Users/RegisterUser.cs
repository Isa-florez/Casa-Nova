using CasaNova.Domain.Entities;
using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Users;

public record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    UserRole Role
) : IRequest<Guid>;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existing = await _uow.Users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new ConflictException("Ya existe una cuenta con ese correo electrónico.");

        var user = User.Create(request.Email, request.FirstName, request.LastName, request.Role);
        user.SetPasswordHash(_passwordHasher.Hash(request.Password));

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return user.Id;
    }
}