FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Firmeza.Test/Firmeza.Test.csproj", "Firmeza.Test/"]
RUN dotnet restore "Firmeza.Test/Firmeza.Test.csproj"
COPY . .
WORKDIR "/src/Firmeza.Test"
RUN dotnet build "./Firmeza.Test.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Firmeza.Test.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.Test.dll"]
