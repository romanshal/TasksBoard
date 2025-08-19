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
COPY ["Services/Authentication/Authentication.API/Authentication.API.csproj", "Services/Authentication/Authentication.API/"]
COPY ["BuildingBlocks/Common.Blocks/Common.Blocks.csproj", "BuildingBlocks/Common.Blocks/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
COPY ["BuildingBlocks/Common.gRPC/Common.gRPC.csproj", "BuildingBlocks/Common.gRPC/"]
COPY ["Services/Authentication/Authentication.Application/Authentication.Application.csproj", "Services/Authentication/Authentication.Application/"]
COPY ["Services/Authentication/Authentication.Domain/Authentication.Domain.csproj", "Services/Authentication/Authentication.Domain/"]
COPY ["Services/Authentication/Authentication.Infrastructure/Authentication.Infrastructure.csproj", "Services/Authentication/Authentication.Infrastructure/"]
RUN dotnet restore "./Services/Authentication/Authentication.API/Authentication.API.csproj"
COPY . .
WORKDIR "/src/Services/Authentication/Authentication.API"
RUN dotnet build "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]