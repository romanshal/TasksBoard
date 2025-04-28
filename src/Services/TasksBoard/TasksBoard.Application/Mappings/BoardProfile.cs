using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardProfile : Profile
    {
        public BoardProfile()
        {
            CreateMap<Board, BoardDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Tag)));

            CreateMap<CreateBoardCommand, Board>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.Tags != null
                    ? src.Tags.Select(tag => new BoardTag { Tag = tag }).ToList()
                    : new List<BoardTag>()));
        }
    }
}
