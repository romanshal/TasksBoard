using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardMemberProfile : Profile
    {
        public BoardMemberProfile()
        {
            CreateMap<BoardMember, BoardMemberDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId.Value))
                .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.Board.OwnerId == src.AccountId))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.BoardMemberPermissions.OrderBy(e => e.BoardPermission.AccessLevel)))
                .ForMember(dest => dest.Nickname, opt => opt.Ignore());
        }
    }
}
