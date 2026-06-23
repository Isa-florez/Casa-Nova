using CasaNova.Domain.Entities;
using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Properties;

public record CreatePropertyCommand(
    string Title,
    string Description,
    string City,
    string Address,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms
) : IRequest<Guid>;

public class CreatePropertyHandler : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreatePropertyHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreatePropertyCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var owner = await _uow.Users.GetByIdAsync(_currentUser.UserId.Value, ct)
            ?? throw new NotFoundException(nameof(User), _currentUser.UserId.Value);

        if (owner.Role != UserRole.Owner)
            throw new UnauthorizedException("Solo los propietarios pueden publicar inmuebles.");

        var property = Property.Create(
            owner.Id,
            request.Title,
            request.Description,
            request.City,
            request.Address,
            request.PricePerNight,
            request.MaxGuests,
            request.Bedrooms
        );

        await _uow.Properties.AddAsync(property, ct);
        await _uow.SaveChangesAsync(ct);

        return property.Id;
    }
}