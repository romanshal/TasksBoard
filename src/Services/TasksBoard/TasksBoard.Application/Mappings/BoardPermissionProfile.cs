using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardPermissionProfile : Profile
    {
        public BoardPermissionProfile()
        {
            CreateMap<BoardPermission, BoardPermissionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value));
        }
    }
}
