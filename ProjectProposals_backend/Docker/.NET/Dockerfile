FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Cache dependency restoration layer
COPY ["ProjectApproval.sln", "./"]
COPY ["ProjectApproval.Api/ProjectApproval.Api.csproj", "ProjectApproval.Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application.UnitTests/Application.UnitTests.csproj", "Application.UnitTests/"]

RUN dotnet restore "ProjectApproval.sln"

COPY . .

WORKDIR "/src/ProjectApproval.Api"
RUN dotnet publish "ProjectApproval.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProjectApproval.Api.dll"]