using AutoMapper;
using Common.Blocks.Constants;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardInviteRequestProfile : Profile
    {
        public BoardInviteRequestProfile()
        {
            CreateMap<CreateBoardInviteRequestCommand, BoardInviteRequest>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)BoardInviteRequestStatuses.Pending));

            CreateMap<BoardInviteRequest, BoardInviteRequestDto>()
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board.Name));
        }
    }
}
