# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos de proyecto (csproj) y restauramos dependencias
# Nota: Ajusta los nombres de las carpetas si difieren ligeramente, me baso en tu imagen
COPY ["banking-api/banking-api.csproj", "banking-api/"]
COPY ["Banking.Application/Banking.Application.csproj", "Banking.Application/"]
COPY ["Banking.Domain/Banking.Domain.csproj", "Banking.Domain/"]
COPY ["Banking.Infrastructure/Banking.Infrastructure.csproj", "Banking.Infrastructure/"]
# Si tienes tests y no quieres correrlos en el build, omítelos aquí
# COPY ["Banking.UnitTests/Banking.UnitTests.csproj", "Banking.UnitTests/"]

RUN dotnet restore "banking-api/banking-api.csproj"

# Copiamos todo el resto del código
COPY . .
WORKDIR "/src/banking-api"
RUN dotnet build "banking-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "banking-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "banking-api.dll"]