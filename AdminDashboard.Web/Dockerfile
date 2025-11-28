# Stage 1 — Restore & Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files for restore
COPY AdminDashboard.Web/*.csproj AdminDashboard.Web/
COPY AdminDashboard.Application/*.csproj AdminDashboard.Application/
COPY AdminDashboard.Domain/*.csproj AdminDashboard.Domain/
COPY AdminDashboard.Infrastructure/*.csproj AdminDashboard.Infrastructure/
COPY AdminDashboard.Identity/*.csproj AdminDashboard.Identity/
COPY ExcelImporter/*.csproj ExcelImporter/
COPY SalePDF/*.csproj SalePDF/
COPY SmtpSettings/*.csproj SmtpSettings/

RUN dotnet restore AdminDashboard.Web/AdminDashboard.Web.csproj

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish AdminDashboard.Web/AdminDashboard.Web.csproj -c Release -o /app/publish

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdminDashboard.Web.dll"]
