using CasaNova.Domain.Entities;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Wishlist;

public record ToggleWishlistCommand(Guid PropertyId) : IRequest<bool>;

public class ToggleWishlistHandler : IRequestHandler<ToggleWishlistCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ToggleWishlistHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ToggleWishlistCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        _ = await _uow.Properties.GetByIdAsync(request.PropertyId, ct)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        var existing = await _uow.Wishlists.GetByUserAndPropertyAsync(
            _currentUser.UserId.Value, request.PropertyId, ct);

        if (existing is not null)
        {
            _uow.Wishlists.Remove(existing);
            await _uow.SaveChangesAsync(ct);
            return false;
        }

        var wishlist = Domain.Entities.Wishlist.Create(_currentUser.UserId.Value, request.PropertyId);
        await _uow.Wishlists.AddAsync(wishlist, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}