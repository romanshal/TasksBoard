using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardNoticeProfile : Profile
    {
        public BoardNoticeProfile()
        {
            CreateMap<CreateBoardNoticeCommand, BoardNotice>();

            CreateMap<BoardNotice, BoardNoticeDto>();
        }
    }
}
