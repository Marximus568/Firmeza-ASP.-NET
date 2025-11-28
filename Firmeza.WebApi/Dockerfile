# Stage 1 — Restore & Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Firmeza.WebApi/Firmeza.WebApi.csproj Firmeza.WebApi/
COPY AdminDashboard.Application/*.csproj AdminDashboard.Application/
COPY AdminDashboard.Domain/*.csproj AdminDashboard.Domain/
COPY AdminDashboard.Infrastructure/*.csproj AdminDashboard.Infrastructure/
COPY AdminDashboard.Identity/*.csproj AdminDashboard.Identity/
COPY AdminDashboard.Web/*.csproj AdminDashboard.Web/

RUN dotnet restore Firmeza.WebApi/Firmeza.WebApi.csproj

COPY . .
RUN dotnet publish Firmeza.WebApi/Firmeza.WebApi.csproj -c Release -o /app/publish

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.WebApi.dll"]
