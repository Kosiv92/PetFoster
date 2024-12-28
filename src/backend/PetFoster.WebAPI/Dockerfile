#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PetFoster.WebAPI/PetFoster.WebAPI.csproj", "PetFoster.WebAPI/"]
COPY ["PetFoster.Application/PetFoster.Application.csproj", "PetFoster.Application/"]
COPY ["PetFoster.Domain/PetFoster.Domain.csproj", "PetFoster.Domain/"]
COPY ["PetFoster.Infrastructure/PetFoster.Infrastructure.csproj", "PetFoster.Infrastructure/"]
RUN dotnet restore "./PetFoster.WebAPI/PetFoster.WebAPI.csproj"
COPY . .
WORKDIR "/src/PetFoster.WebAPI"
RUN dotnet build "./PetFoster.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PetFoster.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetFoster.WebAPI.dll"]