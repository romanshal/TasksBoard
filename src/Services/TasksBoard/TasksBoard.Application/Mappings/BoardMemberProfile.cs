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
                .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.Board.OwnerId == src.AccountId));
        }
    }
}
