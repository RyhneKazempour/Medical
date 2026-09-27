# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["MyApp.sln", "."]

COPY ["src/Shared/Shared.Domain/Shared.Domain.csproj", "src/Shared/Shared.Domain/"]
COPY ["src/Shared/Shared.Application/Shared.Application.csproj", "src/Shared/Shared.Application/"]
COPY ["src/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "src/Shared/Shared.Infrastructure/"]

COPY ["src/Modules/Users/Users.Domain/Users.Domain.csproj", "src/Modules/Users/Users.Domain/"]
COPY ["src/Modules/Users/Users.Application/Users.Application.csproj", "src/Modules/Users/Users.Application/"]
COPY ["src/Modules/Users/Users.Infrastructure/Users.Infrastructure.csproj", "src/Modules/Users/Users.Infrastructure/"]
COPY ["src/Modules/Users/Users.Api/Users.Api.csproj", "src/Modules/Users/Users.Api/"]

COPY ["src/MyApp.Api/MyApp.Api.csproj", "src/MyApp.Api/"]

RUN dotnet restore "MyApp.sln"

COPY . .

WORKDIR "/src/src/MyApp.Api"
RUN dotnet publish "MyApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MyApp.Api.dll"]