using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice
{
    public class CreateBoardNoticeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateBoardNoticeCommandHandler> logger
        ) : IRequestHandler<CreateBoardNoticeCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreateBoardNoticeCommandHandler> _logger = logger;

        public async Task<Guid> Handle(CreateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            var notice = _mapper.Map<BoardNotice>(request);

            await _unitOfWork.GetRepository<BoardNotice>().Add(notice, true, cancellationToken);

            if (notice.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board noticc.");
                throw new ArgumentException(nameof(notice));
            }

            return notice.Id;
        }
    }
}
