# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Lab2_Etapa1_DW_Server_CSharp.csproj

RUN dotnet publish Lab2_Etapa1_DW_Server_CSharp.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Lab2_Etapa1_DW_Server_CSharp.dll"]
