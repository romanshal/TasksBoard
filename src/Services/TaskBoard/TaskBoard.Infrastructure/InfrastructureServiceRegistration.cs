using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Infrastructure.Contexts;
using TaskBoard.Infrastructure.Interfaces.Repositories;
using TaskBoard.Infrastructure.Repositories;

namespace TaskBoard.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskBoardDbContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("TaskBoardDbConnection")!));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddTransient<INoticeRepository, NoticeRepository>();

            return services;
        }
    }
}
