using Authentication.Application.Dtos;
using AutoMapper;

namespace Authentication.API.Mappings
{
    public class UserGrpcProfile : Profile
    {
        public UserGrpcProfile()
        {
            CreateMap<UserInfoDto, Common.gRPC.Protos.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
