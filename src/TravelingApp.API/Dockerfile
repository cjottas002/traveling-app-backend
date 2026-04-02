FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TravelingApp.API/TravelingApp.API.csproj", "src/TravelingApp.API/"]
COPY ["src/TravelingApp.Application/TravelingApp.Application.csproj", "src/TravelingApp.Application/"]
COPY ["src/TravelingApp.Domain/TravelingApp.Domain.csproj", "src/TravelingApp.Domain/"]
COPY ["src/TravelingApp.Infraestructure/TravelingApp.Infraestructure.csproj", "src/TravelingApp.Infraestructure/"]
RUN dotnet restore "./src/TravelingApp.API/TravelingApp.API.csproj"
COPY . .
WORKDIR "/src/src/TravelingApp.API"
RUN dotnet build "./TravelingApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TravelingApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelingApp.API.dll"]
