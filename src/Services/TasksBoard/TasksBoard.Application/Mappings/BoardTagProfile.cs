using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardTagProfile : Profile
    {
        public BoardTagProfile()
        {
            CreateMap<BoardTag, BoardTagDto>();
        }
    }
}
