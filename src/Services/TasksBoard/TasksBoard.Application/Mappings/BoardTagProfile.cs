using AutoMapper;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardTagProfile : Profile
    {
        public BoardTagProfile()
        {
            CreateMap<BoardTag, BoardTagDto>()
                .ReverseMap();
        }
    }
}
