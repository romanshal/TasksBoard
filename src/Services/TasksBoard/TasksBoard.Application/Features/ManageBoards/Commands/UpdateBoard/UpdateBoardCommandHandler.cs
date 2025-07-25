﻿using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public class UpdateBoardCommandHandler(
        ILogger<UpdateBoardCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardCommand, Guid>
    {
        private readonly ILogger<UpdateBoardCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
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
            board.Tags = [.. request.Tags.Select(tag => new BoardTag { Tag = tag })];

            _unitOfWork.GetRepository<Board>().Update(board);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || board.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board with id '{id}'.", board.Id);
                throw new ArgumentException(nameof(board));
            }

            _logger.LogInformation("Board with id '{id}' updated'.", board.Id);

            return board.Id;
        }
    }
}
