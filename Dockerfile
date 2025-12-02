# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy project files
COPY FinalProject.API/*.csproj ./FinalProject.API/
COPY FinalProject.Application/*.csproj ./FinalProject.Application/
COPY FinalProject.Domain/*.csproj ./FinalProject.Domain/
COPY FinalProject.Infrastructure/*.csproj ./FinalProject.Infrastructure/

# Restore dependencies for the main project (this will restore all dependencies)
RUN dotnet restore FinalProject.API/FinalProject.API.csproj

# Copy everything else
COPY . .

# Build and publish
WORKDIR /source/FinalProject.API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port (Render assigns dynamically, but we set default)
EXPOSE 8080

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "FinalProject.API.dll"]
