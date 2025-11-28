# ----------------------------
# 1. Build stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartProxy/SmartProxy.csproj SmartProxy/
COPY DataWarehouse1/DataWarehouse1.csproj DataWarehouse1/
COPY DataWarehouse2/DataWarehouse2.csproj DataWarehouse2/
COPY ClientTester/ClientTester.csproj ClientTester/

RUN dotnet restore SmartProxy/SmartProxy.csproj

COPY . .

RUN dotnet publish SmartProxy/SmartProxy.csproj -c Release -o /app/publish

# ----------------------------
# 2. Runtime stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SmartProxy.dll"]
