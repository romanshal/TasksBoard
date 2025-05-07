using AutoMapper;
using Common.Blocks.Constants;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardAccessRequestProfile : Profile
    {
        public BoardAccessRequestProfile()
        {
            CreateMap<RequestBoardAccessQuery, BoardAccessRequest>().
                ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)BoardAccessRequestStatuses.Pending));

            CreateMap<BoardAccessRequest, BoardAccessRequestDto>()
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board.Name));
        }
    }
}
