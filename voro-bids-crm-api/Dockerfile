FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VoroBidsCrm.API/VoroBidsCrm.API.csproj", "VoroBidsCrm.API/"]
COPY ["VoroBidsCrm.Application/VoroBidsCrm.Application.csproj", "VoroBidsCrm.Application/"]
COPY ["VoroBidsCrm.Contract/VoroBidsCrm.Contract.csproj", "VoroBidsCrm.Contract/"]
COPY ["VoroBidsCrm.Domain/VoroBidsCrm.Domain.csproj", "VoroBidsCrm.Domain/"]
COPY ["VoroBidsCrm.Infrastructure/VoroBidsCrm.Infrastructure.csproj", "VoroBidsCrm.Infrastructure/"]
COPY ["VoroBidsCrm.Shared/VoroBidsCrm.Shared.csproj", "VoroBidsCrm.Shared/"]
RUN dotnet restore "VoroBidsCrm.API/VoroBidsCrm.API.csproj"
COPY . .
WORKDIR "/src/VoroBidsCrm.API"
RUN dotnet build "VoroBidsCrm.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VoroBidsCrm.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoroBidsCrm.API.dll"]
