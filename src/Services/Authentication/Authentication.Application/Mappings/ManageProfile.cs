using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using AutoMapper;

namespace Authentication.Application.Mappings
{
    public class ManageProfile : Profile
    {
        public ManageProfile()
        {
            CreateMap<ApplicationUser, UserInfoDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUserImage, UserImageDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.Extension));
        }
    }
}
