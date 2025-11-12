using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TasksBoard.Infrastructure.Data.Contexts;
using Testcontainers.PostgreSql;

namespace TasksBoard.Tests.Units.Infrastructure
{
    public class InfrastructureTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder().Build();

        public TasksBoardDbContext GetDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            return new(new DbContextOptionsBuilder<TasksBoardDbContext>().UseNpgsql(dbContainer.GetConnectionString()).Options);
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

        public async Task DisposeAsync() => await dbContainer.DisposeAsync();
    }
}
