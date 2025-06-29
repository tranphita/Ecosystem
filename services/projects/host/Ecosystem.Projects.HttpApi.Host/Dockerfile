# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/projects/NuGet.Config", "services/projects/"]
COPY ["services/administration/NuGet.Config", "services/administration/"]
COPY ["services/projects/host/Ecosystem.Projects.HttpApi.Host/Ecosystem.Projects.HttpApi.Host.csproj", "services/projects/host/Ecosystem.Projects.HttpApi.Host/"]
COPY ["shared/Ecosystem.Microservice.Shared/Ecosystem.Microservice.Shared.csproj", "shared/Ecosystem.Microservice.Shared/"]
COPY ["services/administration/src/Ecosystem.Administration.EntityFrameworkCore/Ecosystem.Administration.EntityFrameworkCore.csproj", "services/administration/src/Ecosystem.Administration.EntityFrameworkCore/"]
COPY ["shared/Ecosystem.Shared/Ecosystem.Shared.csproj", "shared/Ecosystem.Shared/"]
COPY ["services/administration/src/Ecosystem.Administration.Domain/Ecosystem.Administration.Domain.csproj", "services/administration/src/Ecosystem.Administration.Domain/"]
COPY ["services/administration/src/Ecosystem.Administration.Domain.Shared/Ecosystem.Administration.Domain.Shared.csproj", "services/administration/src/Ecosystem.Administration.Domain.Shared/"]
COPY ["shared/Ecosystem.Hosting.Shared/Ecosystem.Hosting.Shared.csproj", "shared/Ecosystem.Hosting.Shared/"]
COPY ["shared/Ecosystem.ServiceDefaults/Ecosystem.ServiceDefaults.csproj", "shared/Ecosystem.ServiceDefaults/"]
COPY ["services/projects/src/Ecosystem.Projects.HttpApi/Ecosystem.Projects.HttpApi.csproj", "services/projects/src/Ecosystem.Projects.HttpApi/"]
COPY ["services/projects/src/Ecosystem.Projects.Application.Contracts/Ecosystem.Projects.Application.Contracts.csproj", "services/projects/src/Ecosystem.Projects.Application.Contracts/"]
COPY ["services/projects/src/Ecosystem.Projects.Domain.Shared/Ecosystem.Projects.Domain.Shared.csproj", "services/projects/src/Ecosystem.Projects.Domain.Shared/"]
COPY ["services/projects/src/Ecosystem.Projects.Application/Ecosystem.Projects.Application.csproj", "services/projects/src/Ecosystem.Projects.Application/"]
COPY ["services/projects/src/Ecosystem.Projects.Domain/Ecosystem.Projects.Domain.csproj", "services/projects/src/Ecosystem.Projects.Domain/"]
COPY ["services/projects/src/Ecosystem.Projects.EntityFrameworkCore/Ecosystem.Projects.EntityFrameworkCore.csproj", "services/projects/src/Ecosystem.Projects.EntityFrameworkCore/"]
RUN dotnet restore "./services/projects/host/Ecosystem.Projects.HttpApi.Host/Ecosystem.Projects.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/projects/host/Ecosystem.Projects.HttpApi.Host"
RUN dotnet build "./Ecosystem.Projects.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecosystem.Projects.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecosystem.Projects.HttpApi.Host.dll"]