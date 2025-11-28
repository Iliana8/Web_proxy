# STAGE 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiem doar proiectul SmartProxy
COPY SmartProxy.csproj ./
RUN dotnet restore SmartProxy.csproj

# Copiem restul fișierelor
COPY . ./

RUN dotnet publish SmartProxy.csproj -c Release -o /app

# STAGE 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartProxy.dll"]
