#Publish sdk version
FROM ahmetcagriakca/core-api:latest AS base
WORKDIR /app
EXPOSE 80

#Build sdk version
FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
RUN dotnet restore ConfigurationManager.Api/ConfigurationManager.Api.csproj
WORKDIR /src/ConfigurationManager.Api
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ConfigurationManager.Api.dll"]
