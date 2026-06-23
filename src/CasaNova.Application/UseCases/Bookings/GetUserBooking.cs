using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Bookings;

public record GetUserBookingsQuery : IRequest<IEnumerable<BookingDto>>;

public record BookingDto(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    string PropertyCity,
    string? CoverPhotoUrl,
    DateTime CheckIn,
    DateTime CheckOut,
    int Nights,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAt
);

public class GetUserBookingsHandler : IRequestHandler<GetUserBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetUserBookingsHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var bookings = await _uow.Bookings.GetByUserAsync(_currentUser.UserId.Value, ct);

        return bookings.Select(b => new BookingDto(
            b.Id,
            b.PropertyId,
            b.Property.Title,
            b.Property.City,
            b.Property.Photos.FirstOrDefault(ph => ph.IsCover)?.Url,
            b.CheckIn,
            b.CheckOut,
            b.Nights,
            b.TotalPrice,
            b.Status,
            b.CreatedAt
        ));
    }
}