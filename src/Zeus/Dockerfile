FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.418-buster AS build
WORKDIR /src
COPY ["Zeus/Zeus.csproj", "Zeus/"]
COPY ["Zeus.Storage.Consul/Zeus.Storage.Consul.csproj", "Zeus.Storage.Consul/"]
COPY ["Zeus.Storage/Zeus.Storage.csproj", "Zeus.Storage/"]
COPY ["Zeus.Storage.Faster/Zeus.Storage.Faster.csproj", "Zeus.Storage.Faster/"]
RUN dotnet restore "Zeus/Zeus.csproj"
COPY . .
WORKDIR "/src/Zeus"
RUN dotnet build "Zeus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zeus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zeus.dll"]