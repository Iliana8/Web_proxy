# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiem doar fișierul csproj mai întâi (bună practică)
COPY SmartProxy.csproj .

RUN dotnet restore

# Copiem restul codului
COPY . .

RUN dotnet publish -c Release -o /app


# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartProxy.dll"]
