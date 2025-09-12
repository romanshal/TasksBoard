using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Application.Factories;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public class GetBoardByIdQueryHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardByIdQuery, Result<BoardFullDto>>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<BoardFullDto>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board, BoardId>().GetAsync(BoardId.Of(request.Id), cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{id}' was not found.", request.Id);
                return Result.Failure<BoardFullDto>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.Id}' was not found.");
            }

            var boardDto = _mapper.Map<BoardFullDto>(board);

            await _profileHandler.HandleMany(
            [
                UserProfileMappingFactory.Create(
                    boardDto.InviteRequests,
                    x => x.ToAccountId,
                    (x, u, e) => { x.ToAccountName = u; x.ToAccountEmail = e!; }),

                UserProfileMappingFactory.Create(
                    boardDto.InviteRequests,
                    x => x.FromAccountId,
                    (x, u, _) => { x.FromAccountName = u; }),

                UserProfileMappingFactory.Create(
                    boardDto.AccessRequests,
                    x => x.AccountId,
                    (x, u, e) => { x.AccountName = u; x.AccountEmail = e!; }),

                UserProfileMappingFactory.Create(
                    boardDto.Members,
                    x => x.AccountId,
                    (x, u, _) => { x.Nickname = u; }),
            ], cancellationToken);

            return Result.Success(boardDto);
        }
    }
}
