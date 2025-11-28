# ---------------------------------------------
# 1. Build stage
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy entire solution (important!)
COPY . .

# Restore dependencies
RUN dotnet restore Firmeza.sln

# Build solution
RUN dotnet build Firmeza.sln --configuration Release --no-restore

# ---------------------------------------------
# 2. Test stage (final)
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test

WORKDIR /src

# Copy everything from build stage
COPY --from=build /src .

# Run tests – if any test fails, container exits with non-zero code
CMD ["dotnet", "test", "Firmeza.Test/Firmeza.Test.csproj", "--logger:trx", "--no-build"]
