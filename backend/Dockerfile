# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for caching dotnet restore
COPY ["AssignmentSystem.Api/AssignmentSystem.Api.csproj", "AssignmentSystem.Api/"]
COPY ["AssignmentSystem.Application/AssignmentSystem.Application.csproj", "AssignmentSystem.Application/"]
COPY ["AssignmentSystem.Domain/AssignmentSystem.Domain.csproj", "AssignmentSystem.Domain/"]
COPY ["AssignmentSystem.Infrastructure/AssignmentSystem.Infrastructure.csproj", "AssignmentSystem.Infrastructure/"]

RUN dotnet restore "AssignmentSystem.Api/AssignmentSystem.Api.csproj"

# Copy all source files and publish
COPY . .
WORKDIR "/src/AssignmentSystem.Api"
RUN dotnet publish "AssignmentSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssignmentSystem.Api.dll"]
