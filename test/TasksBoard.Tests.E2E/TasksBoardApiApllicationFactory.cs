using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TasksBoard.Infrastructure.Data.Contexts;
using Testcontainers.PostgreSql;

namespace TasksBoard.Tests.E2E
{
    public class TasksBoardApiApllicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithCleanUp(true)
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TasksBoardDbConnection"] = dbContainer.GetConnectionString(),
                ["OpenSearch:IndexFormat"] = "{0:yyyy.MM.dd}",
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
