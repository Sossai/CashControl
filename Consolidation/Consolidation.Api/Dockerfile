# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Consolidation/Consolidation.Api/Consolidation.Api.csproj", "Consolidation/Consolidation.Api/"]
COPY ["Consolidation/Consolidation.Application/Consolidation.Application.csproj", "Consolidation/Consolidation.Application/"]
COPY ["Consolidation/Consolidation.Infrastructure/Consolidation.Infrastructure.csproj", "Consolidation/Consolidation.Infrastructure/"]
COPY ["Consolidation/Consolidation.Domain/Consolidation.Domain.csproj", "Consolidation/Consolidation.Domain/"]
COPY ["Shared/Shared/Shared.csproj", "Shared/Shared/"]
RUN dotnet restore "./Consolidation/Consolidation.Api/Consolidation.Api.csproj"
COPY . .
WORKDIR "/src/Consolidation/Consolidation.Api"
RUN dotnet build "./Consolidation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Consolidation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consolidation.Api.dll"]