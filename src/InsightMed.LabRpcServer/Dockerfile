FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

WORKDIR /src/src/InsightMed.LabRpcServer
RUN dotnet restore
RUN dotnet publish \
  -c Release \
  -o /app/publish \
  --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "InsightMed.LabRpcServer.dll"]