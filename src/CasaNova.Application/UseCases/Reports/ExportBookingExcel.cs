using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Reports;

public record ExportBookingsExcelQuery(DateTime? From, DateTime? To) : IRequest<byte[]>;

public class ExportBookingsExcelHandler : IRequestHandler<ExportBookingsExcelQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IExcelExportService _excelService;

    public ExportBookingsExcelHandler(IUnitOfWork uow, ICurrentUserService currentUser, IExcelExportService excelService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _excelService = excelService;
    }

    public async Task<byte[]> Handle(ExportBookingsExcelQuery request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var bookings = await _uow.Bookings.GetByOwnerAsync(
            _currentUser.UserId.Value, request.From, request.To, ct);

        return _excelService.ExportBookings(bookings);
    }

    public interface IExcelExportService
    {
        byte[] ExportBookings(IEnumerable<Domain.Entities.Booking> bookings);
    }
}