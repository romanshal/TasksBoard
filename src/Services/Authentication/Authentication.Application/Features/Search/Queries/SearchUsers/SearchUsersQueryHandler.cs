using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Search.Queries.SearchUsers
{
    public class SearchUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<SearchUsersQueryHandler> logger) : IRequestHandler<SearchUsersQuery, IEnumerable<UserInfoDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SearchUsersQueryHandler> _logger = logger;

        public async Task<IEnumerable<UserInfoDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            var searchString = request.Query.Trim().ToLower();

            var users = await _userManager.Users.Where(user => user.UserName!.ToLower().Contains(searchString)).ToListAsync(cancellationToken);

            var usersDto = _mapper.Map<IEnumerable<UserInfoDto>>(users);

            return usersDto;
        }
    }
}
