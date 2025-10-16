using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Extensions;
using Common.Outbox.Abstraction.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice
{
    public class CreateBoardNoticeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxEventFactory outboxFactory,
        ILogger<CreateBoardNoticeCommandHandler> logger) : IRequestHandler<CreateBoardNoticeCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;
        private readonly ILogger<CreateBoardNoticeCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var notice = _mapper.Map<BoardNotice>(request);

                _unitOfWork.GetBoardNoticeRepository().Add(notice);

                var outboxEvent = _outboxFactory.Create(new NewNoticeEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    NoticeId = notice.Id.Value,
                    NoticeDefinition = notice.Definition,
                    AccountId = notice.AuthorId.Value,
                    UsersInterested = [.. board.BoardMembers.Where(member => member.AccountId != AccountId.Of(request.AuthorId)).Select(member => member.AccountId.Value)]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || notice.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't create new board notice to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardNoticeErrors.CantCreate);
                }

                _logger.LogInformation("Board notice with id '{id}' added to board with id '{boardId}'.", notice.Id, request.BoardId);

                return Result.Success(notice.Id.Value);
            }, cancellationToken);
        }
    }
}
