FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GameManager.Server/GameManager.Server.csproj", "GameManager.Server/"]
RUN dotnet restore "GameManager.Server/GameManager.Server.csproj"
COPY . .
WORKDIR "/src/GameManager.Server"
RUN dotnet build "GameManager.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameManager.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "GameManager.Server.dll"]
