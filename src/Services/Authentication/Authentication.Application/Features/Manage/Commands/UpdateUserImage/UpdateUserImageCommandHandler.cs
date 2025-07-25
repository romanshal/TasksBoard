﻿using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.UnitOfWorks;
using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserImage
{
    public class UpdateUserImageCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateUserImageCommandHandler> logger) : IRequestHandler<UpdateUserImageCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateUserImageCommandHandler> _logger = logger;

        public async Task<Guid> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.UserId);
                throw new NotFoundException($"User with id '{request.UserId}' not found.");
            }

            var image = await _unitOfWork.GetApplicationUserImageRepository().GetByUserIdAsync(request.UserId, cancellationToken);
            if (image is null)
            {
                image = new ApplicationUserImage
                {
                    UserId = request.UserId,
                    Image = request.Image,
                    Extension = request.ImageExtension
                };

                _unitOfWork.GetApplicationUserImageRepository().Add(image);
            }
            else
            {
                image.Image = request.Image;
                image.Extension = request.ImageExtension;

                _unitOfWork.GetApplicationUserImageRepository().Update(image);
            }

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || image.Id == Guid.Empty)
            {
                _logger.LogError("Can't update user image with id '{id}'.", user.Id);
                throw new ArgumentException(nameof(image));
            }

            _logger.LogInformation("User image with user id '{id}' was successfully updated.", user.Id);

            return user.Image.Id;
        }
    }
}
