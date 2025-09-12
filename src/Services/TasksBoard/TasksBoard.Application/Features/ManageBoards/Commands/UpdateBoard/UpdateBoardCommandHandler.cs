using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, Result<Guid>>
    {
        private readonly ILogger<UpdateBoardCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBoardRepository _boardRepository;

        public UpdateBoardCommandHandler(
            ILogger<UpdateBoardCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
            this._boardRepository = _unitOfWork.GetBoardRepository();
        }

        public async Task<Result<Guid>> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _boardRepository.GetAsync(BoardId.Of(request.BoardId), token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                board.Name = request.Name;
                board.Description = request.Description;
                board.Public = request.Public;

                if (request.Image is not null)
                {
                    if (board.BoardImage is null)
                    {
                        board.BoardImage = new BoardImage
                        {
                            BoardId = board.Id,
                            Image = request.Image,
                            Extension = request.ImageExtension!
                        };
                    }
                    else
                    {
                        board.BoardImage.Extension = request.ImageExtension!;
                        board.BoardImage.Image = request.Image;
                    }
                }
                else
                {
                    if (board.BoardImage is not null)
                    {
                        board.BoardImage = null;
                    }
                }

                board.Tags.Clear();
                board.Tags = [.. request.Tags.Select(tag => new BoardTag 
                { 
                    BoardId = board.Id,
                    Tag = tag 
                })];

                _boardRepository.Update(board);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || board.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't update board with id '{id}'.", board.Id);
                    return Result.Failure<Guid>(BoardErrors.CantUpdate);
                }

                _logger.LogInformation("Board with id '{id}' updated'.", board.Id);

                return Result.Success(board.Id.Value);
            }, cancellationToken);
        }
    }
}
