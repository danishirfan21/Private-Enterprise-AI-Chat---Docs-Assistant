# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files
COPY EnterpriseAI.sln ./
COPY src/EnterpriseAI.API/EnterpriseAI.API.csproj src/EnterpriseAI.API/
COPY src/EnterpriseAI.Core/EnterpriseAI.Core.csproj src/EnterpriseAI.Core/
COPY src/EnterpriseAI.Infrastructure/EnterpriseAI.Infrastructure.csproj src/EnterpriseAI.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY src/ src/

# Build and publish
WORKDIR /source/src/EnterpriseAI.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "EnterpriseAI.API.dll"]
