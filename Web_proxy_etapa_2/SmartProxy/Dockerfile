# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy complete solution
COPY Web_proxy_etapa_2/WebProxySolution.sln .
COPY Web_proxy_etapa_2/SmartProxy/ SmartProxy/
COPY Web_proxy_etapa_2/DataWarehouse1/ DataWarehouse1/
COPY Web_proxy_etapa_2/DataWarehouse2/ DataWarehouse2/
COPY Web_proxy_etapa_2/ClientTester/ ClientTester/

# Restore NuGet dependencies
RUN dotnet restore WebProxySolution.sln

# Build project
RUN dotnet publish SmartProxy/SmartProxy.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartProxy.dll"]
