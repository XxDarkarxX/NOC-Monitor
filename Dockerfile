# Etapa 1 — build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY MonitorProject.csproj .
RUN dotnet restore MonitorProject.csproj

# Copiar todo el proyecto
COPY . .

# Compilar en modo Release
RUN dotnet publish MonitorProject.csproj -c Release -o /app/out


# Etapa 2 — runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

# Copiar binarios publicados desde el build
COPY --from=build /app/out .

# Copiar config.json en ejecución NO se hace aquí (lo haremos por volumen)
# CMD estándar del monitor
ENTRYPOINT ["dotnet", "MonitorProject.dll"]