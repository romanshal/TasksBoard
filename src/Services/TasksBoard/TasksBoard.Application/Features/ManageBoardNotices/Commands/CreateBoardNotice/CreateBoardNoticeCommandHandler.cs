using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice
{
    public class CreateBoardNoticeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOutboxService outboxService,
        ILogger<CreateBoardNoticeCommandHandler> logger) : IRequestHandler<CreateBoardNoticeCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<CreateBoardNoticeCommandHandler> _logger = logger;

        public async Task<Guid> Handle(CreateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var notice = _mapper.Map<BoardNotice>(request);

            _unitOfWork.GetRepository<BoardNotice>().Add(notice);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || notice.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board notice to board with id '{boardId}'.", request.BoardId);
                throw new ArgumentException(nameof(notice));
            }

            await _outboxService.CreateNewOutboxEvent(new NewNoticeEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                NoticeId = notice.Id,
                NoticeDefinition = notice.Definition,
                AccountId = notice.AuthorId,
                AccountName = notice.AuthorName,
                BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.AuthorId).Select(member => member.AccountId)]
            }, cancellationToken);

            _logger.LogInformation("Board notice with id '{id}' added to board with id '{boardId}'.", notice.Id, request.BoardId);

            return notice.Id;
        }
    }
}
