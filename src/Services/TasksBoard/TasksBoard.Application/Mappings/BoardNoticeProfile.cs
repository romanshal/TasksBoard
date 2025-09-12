using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Mappings
{
    public class BoardNoticeProfile : Profile
    {
        public BoardNoticeProfile()
        {
            CreateMap<CreateBoardNoticeCommand, BoardNotice>()
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => BoardId.Of(src.BoardId)))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(s => false))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Board, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());

            CreateMap<BoardNotice, BoardNoticeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId.Value))
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board.Name))
                .ForMember(dest => dest.AuthorName, opt => opt.Ignore());
        }
    }
}
