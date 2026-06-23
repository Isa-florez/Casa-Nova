using CasaNova.Domain.Entities;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Bookings;

public record CreateBookingCommand(
    Guid PropertyId,
    DateTime CheckIn,
    DateTime CheckOut
) : IRequest<Guid>;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateBookingHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var user = await _uow.Users.GetByIdAsync(_currentUser.UserId.Value, ct)
            ?? throw new NotFoundException(nameof(User), _currentUser.UserId.Value);

        // Verificar KYC antes de permitir la primera reserva
        if (user.NeedsKycForFirstBooking())
            throw new DomainException("Debes completar la validación de identidad antes de realizar una reserva.");

        // Verificar double-booking a nivel de BD (segunda barrera)
        var hasOverlap = await _uow.Bookings.HasOverlapAsync(request.PropertyId, request.CheckIn, request.CheckOut, ct: ct);
        if (hasOverlap)
            throw new ConflictException("El inmueble no está disponible para las fechas seleccionadas.");

        var property = await _uow.Properties.GetWithBookingsAsync(request.PropertyId, ct)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        // Crear booking — las reglas del dominio se aplican aquí
        var booking = Booking.Create(user.Id, property, request.CheckIn, request.CheckOut);

        await _uow.Bookings.AddAsync(booking, ct);

        // Crear notificación in-app
        var notification = Notification.Create(
            user.Id,
            Domain.Enums.NotificationType.BookingConfirmed,
            "Reserva confirmada",
            $"Tu reserva en {property.Title} del {booking.CheckIn:dd/MM/yyyy} al {booking.CheckOut:dd/MM/yyyy} fue confirmada."
        );
        await _uow.Notifications.AddAsync(notification, ct);

        await _uow.SaveChangesAsync(ct);

        return booking.Id;
    }
}