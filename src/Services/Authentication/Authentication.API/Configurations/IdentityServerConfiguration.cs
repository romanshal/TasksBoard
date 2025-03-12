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
                    AllowedCorsOrigins =
                    {
                        //https
                        "http://localhost:5255",
                        "https://localhost:7055",

                        //Docker compose
                        "http://coffeeshop.client.mvc:8080",
                        "https://localhost:9003"
                    },
                    RedirectUris =
                    {
                        //https
                        "http://localhost:5255/signin-oidc",
                        "https://localhost:7055/signin-oidc",

                        //Docker compose
                        "http://coffeeshop.client.mvc:8080/signin-oidc",
                        "https://localhost:9003/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        //https
                        "http://localhost:5255/signout-callback-oidc",
                        "https://localhost:7055/signout-callback-oidc",

                        //Docker compose
                        "http://coffeeshop.client.mvc:8080/signout-callback-oidc",
                        "http://localhost:9003/signout-callback-oidc"
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
