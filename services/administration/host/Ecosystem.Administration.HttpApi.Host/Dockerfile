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
COPY ["services/administration/NuGet.Config", "services/administration/"]
COPY ["services/identity/NuGet.Config", "services/identity/"]
COPY ["services/saas/NuGet.Config", "services/saas/"]
COPY ["services/administration/host/Ecosystem.Administration.HttpApi.Host/Ecosystem.Administration.HttpApi.Host.csproj", "services/administration/host/Ecosystem.Administration.HttpApi.Host/"]
COPY ["shared/Ecosystem.Microservice.Shared/Ecosystem.Microservice.Shared.csproj", "shared/Ecosystem.Microservice.Shared/"]
COPY ["services/administration/src/Ecosystem.Administration.EntityFrameworkCore/Ecosystem.Administration.EntityFrameworkCore.csproj", "services/administration/src/Ecosystem.Administration.EntityFrameworkCore/"]
COPY ["shared/Ecosystem.Shared/Ecosystem.Shared.csproj", "shared/Ecosystem.Shared/"]
COPY ["services/administration/src/Ecosystem.Administration.Domain/Ecosystem.Administration.Domain.csproj", "services/administration/src/Ecosystem.Administration.Domain/"]
COPY ["services/administration/src/Ecosystem.Administration.Domain.Shared/Ecosystem.Administration.Domain.Shared.csproj", "services/administration/src/Ecosystem.Administration.Domain.Shared/"]
COPY ["shared/Ecosystem.Hosting.Shared/Ecosystem.Hosting.Shared.csproj", "shared/Ecosystem.Hosting.Shared/"]
COPY ["shared/Ecosystem.ServiceDefaults/Ecosystem.ServiceDefaults.csproj", "shared/Ecosystem.ServiceDefaults/"]
COPY ["services/identity/src/Ecosystem.IdentityService.Application.Contracts/Ecosystem.IdentityService.Application.Contracts.csproj", "services/identity/src/Ecosystem.IdentityService.Application.Contracts/"]
COPY ["services/identity/src/Ecosystem.IdentityService.Domain.Shared/Ecosystem.IdentityService.Domain.Shared.csproj", "services/identity/src/Ecosystem.IdentityService.Domain.Shared/"]
COPY ["services/identity/src/Ecosystem.IdentityService.EntityFrameworkCore/Ecosystem.IdentityService.EntityFrameworkCore.csproj", "services/identity/src/Ecosystem.IdentityService.EntityFrameworkCore/"]
COPY ["services/identity/src/Ecosystem.IdentityService.Domain/Ecosystem.IdentityService.Domain.csproj", "services/identity/src/Ecosystem.IdentityService.Domain/"]
COPY ["services/saas/src/Ecosystem.SaaS.Application.Contracts/Ecosystem.SaaS.Application.Contracts.csproj", "services/saas/src/Ecosystem.SaaS.Application.Contracts/"]
COPY ["services/administration/src/Ecosystem.Administration.Application.Contracts/Ecosystem.Administration.Application.Contracts.csproj", "services/administration/src/Ecosystem.Administration.Application.Contracts/"]
COPY ["services/saas/src/Ecosystem.SaaS.Domain.Shared/Ecosystem.SaaS.Domain.Shared.csproj", "services/saas/src/Ecosystem.SaaS.Domain.Shared/"]
COPY ["services/administration/src/Ecosystem.Administration.HttpApi/Ecosystem.Administration.HttpApi.csproj", "services/administration/src/Ecosystem.Administration.HttpApi/"]
COPY ["services/administration/src/Ecosystem.Administration.Application/Ecosystem.Administration.Application.csproj", "services/administration/src/Ecosystem.Administration.Application/"]
RUN dotnet restore "./services/administration/host/Ecosystem.Administration.HttpApi.Host/Ecosystem.Administration.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/administration/host/Ecosystem.Administration.HttpApi.Host"
RUN dotnet build "./Ecosystem.Administration.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecosystem.Administration.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecosystem.Administration.HttpApi.Host.dll"]