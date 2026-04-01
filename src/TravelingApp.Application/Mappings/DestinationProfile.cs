using AutoMapper;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Mappings
{
    public class DestinationProfile : Profile
    {
        public DestinationProfile()
        {
            CreateMap<Destination, DestinationDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.CreatedAt).ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.UpdatedAt).ToUnixTimeMilliseconds()));
        }
    }
}
