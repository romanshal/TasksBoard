using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Search.Queries.SearchUsersByQuery
{
    public class SearchUsersByQueryQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<SearchUsersByQueryQueryHandler> logger) : IRequestHandler<SearchUsersByQueryQuery, IEnumerable<UserInfoDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SearchUsersByQueryQueryHandler> _logger = logger;

        public async Task<IEnumerable<UserInfoDto>> Handle(SearchUsersByQueryQuery request, CancellationToken cancellationToken)
        {
            var searchString = request.Query.Trim().ToLower();

            var users = await _userManager.Users.Where(user => user.UserName!.ToLower().Contains(searchString)).ToListAsync(cancellationToken);

            var usersDto = _mapper.Map<IEnumerable<UserInfoDto>>(users);

            return usersDto;
        }
    }
}
