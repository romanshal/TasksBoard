using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardNoticeStatusProfile : Profile
    {
        public BoardNoticeStatusProfile()
        {
            CreateMap<BoardNoticeStatus, BoardNoticeStatusDto>();
        }
    }
}
