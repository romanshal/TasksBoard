using Authentication.Application.Features.Search.Queries.SearchUsersById;
using AutoMapper;
using Common.Blocks.Protos;
using Grpc.Core;
using MediatR;

namespace Authentication.API.Controllers
{
    public class UserProfilesGrpсController(
        IMediator mediator,
        IMapper mapper) : UserProfiles.UserProfilesBase
    {
        public override async Task<ResolveUsersResponse> ResolveUsers(ResolveUsersRequest request, ServerCallContext context)
        {
            var result = await mediator.Send(new SearchUsersByIdQuery
            {
                UserIds = request.UserIds.Select(Guid.Parse)
            });

            var users = mapper.Map<IEnumerable<User>>(result);

            var response = new ResolveUsersResponse();
            response.Users.AddRange(users);

            return response;
        }
    }
}
