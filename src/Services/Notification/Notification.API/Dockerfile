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
COPY ["Services/Notification/Notification.API/Notification.API.csproj", "Services/Notification/Notification.API/"]
COPY ["BuildingBlocks/Common.Hubs/Common.Hubs.csproj", "BuildingBlocks/Common.Hubs/"]
COPY ["BuildingBlocks/Common.Monitoring/Common.Monitoring.csproj", "BuildingBlocks/Common.Monitoring/"]
COPY ["Services/Notification/Notification.Application/Notification.Application.csproj", "Services/Notification/Notification.Application/"]
COPY ["BuildingBlocks/Common.Blocks/Common.Blocks.csproj", "BuildingBlocks/Common.Blocks/"]
COPY ["BuildingBlocks/Common.gRPC/Common.gRPC.csproj", "BuildingBlocks/Common.gRPC/"]
COPY ["BuildingBlocks/Common.Cache/Common.Cache.csproj", "BuildingBlocks/Common.Cache/"]
COPY ["BuildingBlocks/EventBus.Messages.Abstraction/EventBus.Messages.Abstraction.csproj", "BuildingBlocks/EventBus.Messages.Abstraction/"]
COPY ["Services/Notification/Notification.Domain/Notification.Domain.csproj", "Services/Notification/Notification.Domain/"]
COPY ["Services/Notification/Notification.Infrastructure/Notification.Infrastructure.csproj", "Services/Notification/Notification.Infrastructure/"]
RUN dotnet restore "./Services/Notification/Notification.API/Notification.API.csproj"
COPY . .
WORKDIR "/src/Services/Notification/Notification.API"
RUN dotnet build "./Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.API.dll"]