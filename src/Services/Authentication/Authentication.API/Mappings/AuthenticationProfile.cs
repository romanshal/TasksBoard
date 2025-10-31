using Authentication.API.Models.Responses;
using Authentication.Application.Dtos;
using AutoMapper;

namespace Authentication.API.Mappings
{
    public class AuthenticationProfile : Profile
    {
        public AuthenticationProfile()
        {
            CreateMap<AuthenticationDto, AuthenticationResponse>()
                .ForMember(dest => dest.ExpiredAt, opt => opt.MapFrom(src => src.AccessTokenExpiredAt!.Value));
        }
    }
}
