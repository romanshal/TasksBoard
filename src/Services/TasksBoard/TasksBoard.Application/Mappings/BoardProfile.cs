using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardProfile : Profile
    {
        public BoardProfile()
        {
            CreateMap<Board, BoardDto>();

            CreateMap<CreateBoardCommand, Board>();
        }
    }
}
