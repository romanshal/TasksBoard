using AutoMapper;
using Common.Blocks.Constants;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardProfile : Profile
    {
        public BoardProfile()
        {
            CreateMap<Board, BoardDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Tag)))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.BoardMembers.OrderByDescending(member => src.OwnerId == member.AccountId).ThenBy(member => member.Nickname)))
                .ForMember(dest => dest.AccessRequests, opt => opt.MapFrom(src => src.AccessRequests.Where(request => request.Status == (int)BoardAccessRequestStatuses.Pending)))
                .ForMember(dest => dest.InviteRequests, opt => opt.MapFrom(src => src.InviteRequests.Where(request => request.Status == (int)BoardInviteRequestStatuses.Pending)))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.BoardImage.Image))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.BoardImage.Extension));

            CreateMap<Board, BoardForViewDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Tag)))
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.BoardMembers.Count))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.BoardImage.Image))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.BoardImage.Extension))
                .ForMember(dest => dest.IsMember, opt => opt.Ignore());

            CreateMap<CreateBoardCommand, Board>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.Tags != null
                    ? src.Tags.Select(tag => new BoardTag { Tag = tag }).ToList()
                    : new List<BoardTag>()))
                .ForMember(dest => dest.BoardImage, opt => opt.MapFrom(src => src.Image != null ? new BoardImage { Extension = src.ImageExtension!, Image = src.Image } : null))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BoardMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Notices, opt => opt.Ignore())
                .ForMember(dest => dest.AccessRequests, opt => opt.Ignore())
                .ForMember(dest => dest.InviteRequests, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
        }
    }
}
