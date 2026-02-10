# ============================================
# STAGE 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files for better caching
COPY *.slnx ./
COPY src/ProductManagement.Domain/*.csproj ./src/ProductManagement.Domain/
COPY src/ProductManagement.Application/*.csproj ./src/ProductManagement.Application/
COPY src/ProductManagement.Infrastructure/*.csproj ./src/ProductManagement.Infrastructure/
COPY src/ProductManagement.Api/*.csproj ./src/ProductManagement.Api/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/src/ProductManagement.Api
RUN dotnet build -c Release -o /app/build

# ============================================
# STAGE 2: Publish
# ============================================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# STAGE 3: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ProductManagement.Api.dll"]
