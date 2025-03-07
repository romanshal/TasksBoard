using Common.Blocks.Extensions;
using TasksBoard.Application;
using TasksBoard.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.AddConsole();
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGetWithAuth("TasksBoard API");

var connectionString = builder.Configuration.GetConnectionString("TasksBoardDbConnection") ?? throw new InvalidOperationException("Connection string 'TasksBoardDbConnection' not found");
builder.Services
    .AddInfrastructureServices(connectionString)
    .AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TasksBoard.API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
