using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice
{
    public class CreateBoardNoticeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<CreateBoardNoticeCommandHandler> logger) : IRequestHandler<CreateBoardNoticeCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<CreateBoardNoticeCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);

                    //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
                }

                var notice = _mapper.Map<BoardNotice>(request);

                _unitOfWork.GetRepository<BoardNotice>().Add(notice);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || notice.Id == Guid.Empty)
                {
                    _logger.LogError("Can't create new board notice to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardNoticeErrors.CantCreate);

                    //throw new ArgumentException(nameof(notice));
                }

                await _outboxService.CreateNewOutboxEvent(new NewNoticeEvent
                {
                    BoardId = board.Id,
                    BoardName = board.Name,
                    NoticeId = notice.Id,
                    NoticeDefinition = notice.Definition,
                    AccountId = notice.AuthorId,
                    BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.AuthorId).Select(member => member.AccountId)]
                }, token);

                _logger.LogInformation("Board notice with id '{id}' added to board with id '{boardId}'.", notice.Id, request.BoardId);

                return Result.Success(notice.Id);
            }, cancellationToken);
        }
    }
}
