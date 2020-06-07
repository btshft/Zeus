FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Zeus.v2/Zeus.v2.csproj", "Zeus.v2/"]
COPY ["Zeus.Storage.Consul/Zeus.Storage.Consul.csproj", "Zeus.Storage.Consul/"]
COPY ["Zeus.Storage/Zeus.Storage.csproj", "Zeus.Storage/"]
RUN dotnet restore "Zeus.v2/Zeus.v2.csproj"
COPY . .
WORKDIR "/src/Zeus.v2"
RUN dotnet build "Zeus.v2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zeus.v2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zeus.v2.dll"]