# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/TasksBoard/TasksBoard.API/TasksBoard.API.csproj", "Services/TasksBoard/TasksBoard.API/"]
COPY ["Services/Authentication/Authentication.Domain/Authentication.Domain.csproj", "Services/Authentication/Authentication.Domain/"]
COPY ["BuildingBlocks/Common.Blocks/Common.Blocks.csproj", "BuildingBlocks/Common.Blocks/"]
COPY ["Services/TasksBoard/TasksBoard.Application/TasksBoard.Application.csproj", "Services/TasksBoard/TasksBoard.Application/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
COPY ["Services/TasksBoard/TasksBoard.Domain/TasksBoard.Domain.csproj", "Services/TasksBoard/TasksBoard.Domain/"]
COPY ["Services/TasksBoard/TasksBoard.Infrastructure/TasksBoard.Infrastructure.csproj", "Services/TasksBoard/TasksBoard.Infrastructure/"]
RUN dotnet restore "./Services/TasksBoard/TasksBoard.API/TasksBoard.API.csproj"
COPY . .
WORKDIR "/src/Services/TasksBoard/TasksBoard.API"
RUN dotnet build "./TasksBoard.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TasksBoard.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TasksBoard.API.dll"]