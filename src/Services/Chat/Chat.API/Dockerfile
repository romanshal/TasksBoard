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
COPY ["Services/Chat/Chat.API/Chat.API.csproj", "Services/Chat/Chat.API/"]
COPY ["BuildingBlocks/Common.Blocks/Common.Blocks.csproj", "BuildingBlocks/Common.Blocks/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
COPY ["BuildingBlocks/Common.Monitoring/Common.Monitoring.csproj", "BuildingBlocks/Common.Monitoring/"]
COPY ["Services/Chat/Chat.Application/Chat.Application.csproj", "Services/Chat/Chat.Application/"]
COPY ["BuildingBlocks/Common.gRPC/Common.gRPC.csproj", "BuildingBlocks/Common.gRPC/"]
COPY ["BuildingBlocks/Common.Cache/Common.Cache.csproj", "BuildingBlocks/Common.Cache/"]
COPY ["Services/TasksBoard/TasksBoard.Domain/TasksBoard.Domain.csproj", "Services/TasksBoard/TasksBoard.Domain/"]
COPY ["Services/Chat/Chat.Domain/Chat.Domain.csproj", "Services/Chat/Chat.Domain/"]
COPY ["Services/Chat/Chat.Infrastructure/Chat.Infrastructure.csproj", "Services/Chat/Chat.Infrastructure/"]
RUN dotnet restore "./Services/Chat/Chat.API/Chat.API.csproj"
COPY . .
WORKDIR "/src/Services/Chat/Chat.API"
RUN dotnet build "./Chat.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chat.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat.API.dll"]