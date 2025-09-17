using AutoMapper;
using Common.Blocks.ValueObjects;
using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Mappings
{
    public class BoardProfile : Profile
    {
        public BoardProfile()
        {
            CreateMap<Board, BoardDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId.Value))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.BoardTags.Select(tag => tag.Tag)))
                //.ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.BoardMembers.OrderByDescending(member => src.OwnerId == member.AccountId)))
                //.ForMember(dest => dest.AccessRequests, opt => opt.MapFrom(src => src.AccessRequests.Where(request => request.Status == (int)BoardAccessRequestStatuses.Pending)))
                //.ForMember(dest => dest.InviteRequests, opt => opt.MapFrom(src => src.InviteRequests.Where(request => request.Status == (int)BoardInviteRequestStatuses.Pending)))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.BoardImage.Image))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.BoardImage.Extension))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public));

            CreateMap<Board, BoardForViewDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.BoardTags.Select(tag => tag.Tag)))
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.BoardMembers.Count))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.BoardImage.Image))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.BoardImage.Extension))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public))
                .ForMember(dest => dest.IsMember, opt => opt.Ignore());

            CreateMap<Board, BoardFullDto>()
                .IncludeBase<Board, BoardDto>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.BoardMembers.OrderByDescending(member => src.OwnerId == member.AccountId)))
                .ForMember(dest => dest.AccessRequests, opt => opt.MapFrom(src => src.BoardAccessRequests.Where(request => request.Status == (int)BoardAccessRequestStatuses.Pending)))
                .ForMember(dest => dest.InviteRequests, opt => opt.MapFrom(src => src.BoardInviteRequests.Where(request => request.Status == (int)BoardInviteRequestStatuses.Pending)));

            CreateMap<CreateBoardCommand, Board>()
                .ForMember(dest => dest.BoardTags, opt => opt.MapFrom(src =>
                src.Tags != null
                    ? src.Tags.Select(tag => new BoardTag { Tag = tag }).ToList()
                    : new List<BoardTag>()))
                .ForMember(dest => dest.BoardImage, opt => opt.MapFrom(src => src.Image != null ? new BoardImage { Extension = src.ImageExtension!, Image = src.Image } : null))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => AccountId.Of(src.OwnerId)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BoardMembers, opt => opt.Ignore())
                .ForMember(dest => dest.BoardNotices, opt => opt.Ignore())
                .ForMember(dest => dest.BoardAccessRequests, opt => opt.Ignore())
                .ForMember(dest => dest.BoardInviteRequests, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
        }
    }
}
