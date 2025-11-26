FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Firmeza.WebApi/Firmeza.WebApi.csproj", "Firmeza.WebApi/"]
COPY ["AdminDashboard.Domain/AdminDashboard.Domain.csproj", "AdminDashboard.Domain/"]
COPY ["AdminDashboard.Infrastructure/AdminDashboard.Infrastructure.csproj", "AdminDashboard.Infrastructure/"]
COPY ["AdminDashboard.Application/AdminDashboard.Application.csproj", "AdminDashboard.Application/"]
COPY ["SalePDF/SalePDF.csproj", "SalePDF/"]
COPY ["AdminDashboard.Identity/AdminDashboard.Identity.csproj", "AdminDashboard.Identity/"]
COPY ["SmtpSettings/SmtpSettings.csproj", "SmtpSettings/"]
COPY ["AdminDashboard.Web/AdminDashboard.Web.csproj", "AdminDashboard.Web/"]
COPY ["ExcelImporter/ExcelImporter.csproj", "ExcelImporter/"]
RUN dotnet restore "Firmeza.WebApi/Firmeza.WebApi.csproj"
COPY . .
WORKDIR "/src/Firmeza.WebApi"
RUN dotnet build "./Firmeza.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Firmeza.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.WebApi.dll"]
