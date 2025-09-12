using AutoMapper;
using Common.Blocks.Constants;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Mappings
{
    public class BoardInviteRequestProfile : Profile
    {
        public BoardInviteRequestProfile()
        {
            CreateMap<CreateBoardInviteRequestCommand, BoardInviteRequest>()
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => BoardId.Of(src.BoardId)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)BoardInviteRequestStatuses.Pending))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Board, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());

            CreateMap<BoardInviteRequest, BoardInviteRequestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId.Value))
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board.Name))
                .ForMember(dest => dest.ToAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.ToAccountEmail, opt => opt.Ignore())
                .ForMember(dest => dest.FromAccountName, opt => opt.Ignore());
        }
    }
}
