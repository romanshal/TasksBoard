using AutoMapper;
using Chat.Application.DTOs;
using Chat.Domain.Constants.Errors.DomainErrors;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateBoardMessageCommandHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<UpdateBoardMessageCommand, Result<BoardMessageDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateBoardMessageCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<BoardMessageDto>> Handle(UpdateBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = await _unitOfWork.GetBoardMessagesRepository().GetAsync(request.BoardMessageId);
            if (boardMessage is null)
            {
                _logger.LogWarning("Board message with id '{boardMessageId}' was not found.", request.BoardMessageId);
                return Result.Failure<BoardMessageDto>(BoardMessageErrors.NotFound);

                //throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.Message = request.Message;

            _unitOfWork.GetBoardMessagesRepository().Update(boardMessage);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || boardMessage.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board message with id '{messageId}' for board with id '{boardId}'.", request.BoardMessageId, request.BoardId);
                return Result.Failure<BoardMessageDto>(BoardMessageErrors.CantUpdate);

                //throw new ArgumentException(nameof(boardMessage));
            }

            var boardMessageDto = _mapper.Map<BoardMessageDto>(boardMessage);

            var userProfile = await _profileService.ResolveAsync(boardMessageDto.AccountId, cancellationToken);

            if (userProfile is not null)
            {
                boardMessageDto.MemberNickname = userProfile.Username;
            }

            _logger.LogInformation("Board message with id '{id}' was updated in board with id '{boardId}'.", boardMessage.Id, request.BoardId);

            return Result.Success(boardMessageDto);
        }
    }
}
