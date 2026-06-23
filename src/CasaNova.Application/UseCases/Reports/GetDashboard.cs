using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Reports;

public record GetDashboardQuery : IRequest<DashboardDto>;

public record DashboardDto(
    int TotalProperties,
    int TotalBookings,
    int ActiveBookings,
    int CancelledBookings,
    decimal TotalRevenue,
    IEnumerable<BookingsByMonthDto> BookingsByMonth
);

public record BookingsByMonthDto(int Year, int Month, int Count, decimal Revenue);

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var properties = await _uow.Properties.GetByOwnerAsync(_currentUser.UserId.Value, ct);
        var bookings = await _uow.Bookings.GetByOwnerAsync(_currentUser.UserId.Value, null, null, ct);

        var bookingsList = bookings.ToList();

        var byMonth = bookingsList
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => new { b.CheckIn.Year, b.CheckIn.Month })
            .Select(g => new BookingsByMonthDto(
                g.Key.Year,
                g.Key.Month,
                g.Count(),
                g.Sum(b => b.TotalPrice)
            ))
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();

        return new DashboardDto(
            properties.Count(),
            bookingsList.Count,
            bookingsList.Count(b => b.Status == BookingStatus.Confirmed),
            bookingsList.Count(b => b.Status == BookingStatus.Cancelled),
            bookingsList.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TotalPrice),
            byMonth
        );
    }
}