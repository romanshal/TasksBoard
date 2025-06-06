using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.UnitOfWorks;
using AutoMapper;
using Common.Blocks.Constants;
using Common.Blocks.Entities;
using Common.Blocks.Exceptions;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateUserInfoCommandHandler> logger) : IRequestHandler<UpdateUserInfoCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateUserInfoCommandHandler> _logger = logger;

        public async Task<Guid> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning($"User with id {request.UserId} not found.");
                throw new NotFoundException($"User with id {request.UserId} not found.");
            }

            user.UserName = request.Username;
            user.Email = request.Email;
            user.FirstName = request.Firstname;
            user.Surname = request.Surname;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogCritical($"Can't update user info with id: {user.Id}. Errors: {string.Join("; ", result.Errors)}.");
                throw new Exception($"Can't update user info with id: {user.Id}. Errors: {string.Join("; ", result.Errors)}.");
            }

            var updateEvent = new UpdateAccountInfoEvent 
            {
                AccountId = user.Id,
                AccountName = user.UserName,
                AccountEmail = user.Email,
                AccountFirstname = user.FirstName,
                AccountSurname = user.Surname
            };

            await _unitOfWork.GetRepository<OutboxEvent>().Add(new OutboxEvent
            {
                EventType = updateEvent.GetType().Name!,
                Payload = JsonSerializer.Serialize(updateEvent),
                Status = OutboxEventStatuses.Created
            }, true, cancellationToken);

            _logger.LogInformation($"User with id {user.Id} was successfully updated.");

            return user.Id;
        }
    }
}
