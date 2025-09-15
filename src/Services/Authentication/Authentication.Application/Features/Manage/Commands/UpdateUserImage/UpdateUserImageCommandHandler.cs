using Authentication.Domain.Constants.ManageErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserImage
{
    public class UpdateUserImageCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserImageCommandHandler> logger) : IRequestHandler<UpdateUserImageCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateUserImageCommandHandler> _logger = logger;

        public async Task<Result> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.Id);
                return Result.Failure(ManageErrors.UserNotFound);
            }

            var image = await _unitOfWork.GetApplicationUserImageRepository().GetByUserIdAsync(request.Id, cancellationToken);
            if (image is null)
            {
                image = new ApplicationUserImage
                {
                    UserId = request.Id,
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
            if (affectedRows == 0 || image.Id.Value == Guid.Empty)
            {
                _logger.LogError("Can't update user image with id '{id}'.", user.Id);
                return Result.Failure(ManageErrors.CantUpdatePassword);
            }

            _logger.LogInformation("User image with user id '{id}' was successfully updated.", user.Id);

            return Result.Success();
        }
    }
}
