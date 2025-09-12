using AutoMapper;
using Common.Blocks.Constants;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Mappings
{
    public class BoardAccessRequestProfile : Profile
    {
        public BoardAccessRequestProfile()
        {
            CreateMap<RequestBoardAccessCommand, BoardAccessRequest>()
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => BoardId.Of(src.BoardId)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)BoardAccessRequestStatuses.Pending))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Board, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());

            CreateMap<BoardAccessRequest, BoardAccessRequestDto>()
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board.Name))
                .ForMember(dest => dest.AccountName, opt => opt.Ignore())
                .ForMember(dest => dest.AccountEmail, opt => opt.Ignore());
        }
    }
}
