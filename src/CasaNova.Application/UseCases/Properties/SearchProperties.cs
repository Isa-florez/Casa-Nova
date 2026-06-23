using CasaNova.Domain.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.Properties;

public record SearchPropertiesQuery(string? City, DateTime? CheckIn, DateTime? CheckOut) : IRequest<IEnumerable<PropertyDto>>;

public record PropertyDto(
    Guid Id,
    string Title,
    string Description,
    string City,
    string Address,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms,
    string? CoverPhotoUrl
);

public class SearchPropertiesHandler : IRequestHandler<SearchPropertiesQuery, IEnumerable<PropertyDto>>
{
    private readonly IUnitOfWork _uow;

    public SearchPropertiesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<PropertyDto>> Handle(SearchPropertiesQuery request, CancellationToken ct)
    {
        var properties = await _uow.Properties.SearchAsync(request.City, request.CheckIn, request.CheckOut, ct);

        return properties.Select(p => new PropertyDto(
            p.Id,
            p.Title,
            p.Description,
            p.City,
            p.Address,
            p.PricePerNight,
            p.MaxGuests,
            p.Bedrooms,
            p.Photos.FirstOrDefault(ph => ph.IsCover)?.Url
        ));
    }
}