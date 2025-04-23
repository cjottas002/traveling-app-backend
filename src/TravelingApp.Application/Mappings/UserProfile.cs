using AutoMapper;
using TravelingApp.Application.Features.Users.Models;
using TravelingApp.Domain.Entities;
namespace TravelingApp.Application.Mapping
{

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.UpdatedAt).ToUnixTimeMilliseconds()));
        }
    }
}
