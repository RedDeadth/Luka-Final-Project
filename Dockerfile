FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FinalProject.API/FinalProject.API.csproj", "FinalProject.API/"]
COPY ["FinalProject.Application/FinalProject.Application.csproj", "FinalProject.Application/"]
COPY ["FinalProject.Domain/FinalProject.Domain.csproj", "FinalProject.Domain/"]
COPY ["FinalProject.Infrastructure/FinalProject.Infrastructure.csproj", "FinalProject.Infrastructure/"]

RUN dotnet restore "FinalProject.API/FinalProject.API.csproj"

COPY . .

WORKDIR "/src/FinalProject.API"
RUN dotnet build "FinalProject.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinalProject.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "FinalProject.API.dll"]
