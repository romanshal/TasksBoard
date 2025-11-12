using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TasksBoard.Infrastructure.Data.Contexts;
using Testcontainers.PostgreSql;

namespace TasksBoard.Tests.E2E
{
    public class TasksBoardApiApllicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly Guid _userId = Guid.Parse("56114028-b282-45b3-b6ca-89982965e4c8");
        private readonly string _username = "Test";
        private readonly JsonSerializerSettings _jsonSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        };

        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithCleanUp(true)
            .Build();

        public TasksBoardDbContext GetDbContext() => new(new DbContextOptionsBuilder<TasksBoardDbContext>()
            .UseNpgsql(dbContainer.GetConnectionString()).Options);

        public JsonSerializerSettings GetJsonSettings() => _jsonSettings;

        public (Guid userId, string username) GetUserCredentials() => (_userId, _username);

        public AuthenticationHeaderValue GetAuthentication() => new("Bearer", this.CreateToken());

        public string CreateToken()
        {
            var (userId, username) = GetUserCredentials();
            var configuration = this.Services.GetService<IConfiguration>()!;

            string secretKey = configuration["Authentication:Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentionals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.Email, "test@gmail.com"),
                    new(ClaimTypes.NameIdentifier, userId.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ]),
                Expires = DateTime.Now.AddMinutes(int.Parse(configuration["Authentication:Jwt:ExpirationInMinutes"]!)),
                SigningCredentials = credentionals,
                Issuer = configuration["Authentication:Jwt:Issuer"],
                Audience = configuration["Authentication:Jwt:Audience"],
            };

            var handler = new JsonWebTokenHandler();

            return handler.CreateToken(tokenDescriptor);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:TasksBoardDbConnection"] = dbContainer.GetConnectionString(),
                ["OpenSearch:IndexFormat"] = "{0:yyyy.MM.dd}",
                ["Authentication:Jwt:Secret"] = "secretsecretsecretsecretsecretsecret",
                ["Authentication:Jwt:Issuer"] = "https://localhost:7148",
                ["Authentication:Jwt:ExpirationInMinutes"] = "480",
                ["Authentication:Jwt:Audience"] = "https://localhost:7227"
            })
            .Build();

            builder.UseConfiguration(configuration);
            builder.ConfigureLogging(cfg => cfg.ClearProviders());

            //base.ConfigureWebHost(builder);
        }

        public async Task InitializeAsync()
        {
            await dbContainer.StartAsync();

            var migrationAssembly = typeof(TasksBoardDbContext)
                .GetTypeInfo()
                .Assembly
                .GetName()
                .Name;

            var options = new DbContextOptionsBuilder<TasksBoardDbContext>()
                .UseNpgsql(dbContainer.GetConnectionString(), npqsqlOptions =>
                    npqsqlOptions.MigrationsAssembly(migrationAssembly))
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
                .Options;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            using var tasksBoardDbContext = new TasksBoardDbContext(options);

            await tasksBoardDbContext.Database.MigrateAsync();
        }

        async Task IAsyncLifetime.DisposeAsync() => await dbContainer.DisposeAsync();
    }
}
