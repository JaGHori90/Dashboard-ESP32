# Build context must be the repository root (needed so Core/Persistence/Utils are reachable):
#   docker build -f ASP.Net/Api/Dockerfile -t dashboard-esp32-api .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ASP.Net/Core/Core.csproj ASP.Net/Core/
COPY ASP.Net/Utils/Utils.csproj ASP.Net/Utils/
COPY ASP.Net/Persistence/Persistence.csproj ASP.Net/Persistence/
COPY ASP.Net/Api/WebApi.csproj ASP.Net/Api/
RUN dotnet restore ASP.Net/Api/WebApi.csproj

COPY ASP.Net/Core/ ASP.Net/Core/
COPY ASP.Net/Utils/ ASP.Net/Utils/
COPY ASP.Net/Persistence/ ASP.Net/Persistence/
COPY ASP.Net/Api/ ASP.Net/Api/

RUN dotnet publish ASP.Net/Api/WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
