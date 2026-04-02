FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TravelingApp.Ui/TravelingApp.Ui.csproj", "src/TravelingApp.Ui/"]
COPY ["src/TravelingApp.Application/TravelingApp.Application.csproj", "src/TravelingApp.Application/"]
COPY ["src/TravelingApp.Domain/TravelingApp.Domain.csproj", "src/TravelingApp.Domain/"]
COPY ["src/TravelingApp.Infraestructure/TravelingApp.Infraestructure.csproj", "src/TravelingApp.Infraestructure/"]
RUN dotnet restore "./src/TravelingApp.Ui/TravelingApp.Ui.csproj"
COPY . .
WORKDIR "/src/src/TravelingApp.Ui"
RUN dotnet build "./TravelingApp.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TravelingApp.Ui.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelingApp.Ui.dll"]
