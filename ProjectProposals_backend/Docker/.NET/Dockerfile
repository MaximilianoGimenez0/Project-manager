# ==============================================================================
# Dockerfile - ASP.NET Core 8 Web API
# Compilación multi-etapa para optimizar tamaño y rendimiento de la imagen
# ==============================================================================

# 1. Etapa de compilación con SDK de .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y proyectos para aprovechar la caché de Docker en la restauración de paquetes
COPY ["ProjectApproval.sln", "./"]
COPY ["ProjectApproval.Api/ProjectApproval.Api.csproj", "ProjectApproval.Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application.UnitTests/Application.UnitTests.csproj", "Application.UnitTests/"]

RUN dotnet restore "ProjectApproval.sln"

# Copiar todo el código fuente del proyecto
COPY . .

# Compilar y publicar la API en Release
WORKDIR "/src/ProjectApproval.Api"
RUN dotnet publish "ProjectApproval.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# 2. Etapa de ejecución con el Runtime ligero de ASP.NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Exponer el puerto por defecto de ASP.NET Core 8
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Copiar binarios compilados
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProjectApproval.Api.dll"]