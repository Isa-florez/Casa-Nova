using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using CasaNova.Domain.Entities;
using MediatR;

namespace CasaNova.Application.UseCases.Bookings;

public record CancelBookingCommand(Guid BookingId) : IRequest;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CancelBookingHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var booking = await _uow.Bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        if (booking.UserId != _currentUser.UserId.Value)
            throw new UnauthorizedException("No puedes cancelar una reserva que no te pertenece.");

        booking.Cancel();

        var notification = Notification.Create(
            booking.UserId,
            NotificationType.BookingCancelled,
            "Reserva cancelada",
            $"Tu reserva en {booking.Property?.Title ?? booking.PropertyId.ToString()} ha sido cancelada."
        );
        await _uow.Notifications.AddAsync(notification, ct);

        await _uow.SaveChangesAsync(ct);
    }
}