using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Domain.Entities;
using MediatR;

namespace CasaNova.Application.UseCases.Properties;

public record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyDetailDto>;

public record PropertyDetailDto(
    Guid Id,
    string Title,
    string Description,
    string City,
    string Address,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms,
    Guid OwnerId,
    IEnumerable<string> PhotoUrls
);

public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, PropertyDetailDto>
{
    private readonly IUnitOfWork _uow;

    public GetPropertyByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PropertyDetailDto> Handle(GetPropertyByIdQuery request, CancellationToken ct)
    {
        var property = await _uow.Properties.GetWithBookingsAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Property), request.Id);

        return new PropertyDetailDto(
            property.Id,
            property.Title,
            property.Description,
            property.City,
            property.Address,
            property.PricePerNight,
            property.MaxGuests,
            property.Bedrooms,
            property.OwnerId,
            property.Photos.OrderBy(p => p.Order).Select(p => p.Url)
        );
    }
}