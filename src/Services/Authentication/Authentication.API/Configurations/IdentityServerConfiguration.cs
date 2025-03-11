using IdentityModel;
using IdentityServer8;
using IdentityServer8.Models;

namespace Authentication.API.Configurations
{
    public class IdentityServerConfiguration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("TasksBoardAPI", "Web API"),
            ];

        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                new("TasksBoardAPI", "Web API", [JwtClaimTypes.Name])
                {
                    Scopes = { "TasksBoardAPI" }
                },
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client {
                    ClientId = "angular.client",
                    ClientName = "Angular client frontend",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("secret".ToSha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TasksBoardAPI"
                    },
                    AllowedCorsOrigins = {

                    },
                    RedirectUris = {

                    },
                    PostLogoutRedirectUris = {

                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 86400,
                }
            ];
    }
}
