FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY apps/api/Teleport.Api.csproj apps/api/
RUN dotnet restore apps/api/Teleport.Api.csproj

COPY apps/api/ apps/api/
WORKDIR /src/apps/api
RUN dotnet publish Teleport.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Teleport.Api.dll"]
