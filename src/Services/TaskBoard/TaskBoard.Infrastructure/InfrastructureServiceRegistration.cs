using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Domain.Interfaces.UnitOfWorks;
using TaskBoard.Infrastructure.Data.Contexts;
using TaskBoard.Infrastructure.UnitOfWorks;

namespace TaskBoard.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskBoardDbContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("TaskBoardDbConnection")!));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
