using AutoMapper;
using Chat.Application.DTOs;
using Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage;
using Chat.Domain.Entities;

namespace Chat.Application.Mappings
{
    public class BoardMessageProfile : Profile
    {
        public BoardMessageProfile()
        {
            CreateMap<BoardMessage, BoardMessageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.LastModifiedAt));

            CreateMap<CreateBoardMessageCommand, BoardMessage>();
        }
    }
}
