using Authentication.API.Models.Responses;
using Authentication.Application.Dtos;
using AutoMapper;

namespace Authentication.API.Mappings
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            CreateMap<UserInfoDto, SearchResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
