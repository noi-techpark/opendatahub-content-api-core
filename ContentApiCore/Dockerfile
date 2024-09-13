FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DataModel/DataModel.csproj DataModel/
COPY Helper/Helper.csproj Helper/
COPY OdhNotifier/OdhNotifier.csproj OdhNotifier/
COPY PushServer/PushServer.csproj EBMS/
COPY JsonLDTransformer/JsonLDTransformer.csproj JsonLDTransformer/
COPY ContentApiCore/ContentApiCore.csproj ContentApiCore/
RUN dotnet restore ContentApiCore/ContentApiCore.csproj
COPY . .
WORKDIR /src/ContentApiCore
RUN dotnet build ContentApiCore.csproj -c Release -o /app

FROM build AS test
WORKDIR /src
RUN dotnet test --filter Category!=Integration ContentApiCore.sln

FROM build AS publish
RUN dotnet publish ContentApiCore.csproj -c Release -o /app/publish
# Install dotnet debug tools
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-gcdump

FROM base AS final
# Copy dotnet-tools
WORKDIR /tools
COPY --from=publish /tools .
WORKDIR /app
COPY --from=publish /app/publish .
COPY ContentApiCore/xml/RequestInterceptor.xml xml/
COPY ContentApiCore/appsettings.Production.json appsettings.json

# Expose the API port
EXPOSE 80

ENTRYPOINT ["dotnet", "ContentApiCore.dll"]
