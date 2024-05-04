FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
ARG version=0.1-dev
WORKDIR /src
COPY ["src/GameManager.Server/GameManager.Server.csproj", "GameManager.Server/"]
RUN dotnet restore "GameManager.Server/GameManager.Server.csproj"
COPY ./src ./
RUN dotnet build "GameManager.Server/GameManager.Server.csproj" -c Release /p:InformationalVersion=$version -o /app/build

RUN dotnet publish "GameManager.Server/GameManager.Server.csproj" -c Release /p:InformationalVersion=$version -o /app/publish

FROM node:18 as build-web
WORKDIR /src

COPY ["web/package.json", "web/package-lock.json", "./"]
RUN npm install

COPY ./web ./
RUN npm run build --configuration=production

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

COPY --from=build-api /app/publish .
COPY --from=build-web /src/dist/game-manager ./wwwroot

VOLUME "/var/game-manager/"
ENV ASPNETCORE_URLS="http://*:5000"
EXPOSE 5000
ENTRYPOINT ["dotnet", "GameManager.Server.dll"]