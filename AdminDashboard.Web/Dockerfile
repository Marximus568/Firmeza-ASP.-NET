FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AdminDashboard.Web/AdminDashboard.Web.csproj AdminDashboard.Web/
RUN dotnet restore AdminDashboard.Web/AdminDashboard.Web.csproj

COPY . .
RUN dotnet publish AdminDashboard.Web/AdminDashboard.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdminDashboard.Web.dll"]
