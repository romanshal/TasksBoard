using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Application.Features.Search.Queries.SearchUsersById
{
    public class SearchUsersByIdQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper) : IRequestHandler<SearchUsersByIdQuery, IEnumerable<UserInfoDto>>
    {
        public async Task<IEnumerable<UserInfoDto>> Handle(SearchUsersByIdQuery request, CancellationToken cancellationToken) =>
            await userManager.Users
                .Where(user => request.UserIds.Contains(user.Id))
                .ProjectTo<UserInfoDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

    }
}
