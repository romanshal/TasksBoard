using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardMemberPermissionProfile : Profile
    {
        public BoardMemberPermissionProfile()
        {
            CreateMap<BoardMemberPermission, BoardMemberPermissionDto>()
                .ForMember(dest => dest.BoardPermissionName, opt => opt.MapFrom(src => src.BoardPermission.Name))
                .ForMember(dest => dest.BoardPermissionDescription, opt => opt.MapFrom(src => src.BoardPermission.Description));
        }
    }
}
