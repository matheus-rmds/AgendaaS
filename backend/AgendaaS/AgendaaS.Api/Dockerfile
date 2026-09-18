FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AgendaaS.Api/AgendaaS.Api.csproj", "AgendaaS.Api/"]
COPY ["AgendaaS.Application/AgendaaS.Application.csproj", "AgendaaS.Application/"]
COPY ["AgendaaS.Infrastructure/AgendaaS.Infrastructure.csproj", "AgendaaS.Infrastructure/"]
COPY ["AgendaaS.Domain/AgendaaS.Domain.csproj", "AgendaaS.Domain/"]
COPY ["AgendaaS.Shared/AgendaaS.Shared.csproj", "AgendaaS.Shared/"]
RUN dotnet restore "AgendaaS.Api/AgendaaS.Api.csproj"

COPY . .
WORKDIR /src/AgendaaS.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AgendaaS.Api.dll"]
