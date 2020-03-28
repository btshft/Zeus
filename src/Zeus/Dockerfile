ARG runtime="3.1-buster-slim"
ARG sdk="3.1-buster"

FROM mcr.microsoft.com/dotnet/core/aspnet:${runtime} AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:${sdk} AS build
ARG configuration=Release
WORKDIR /src
COPY ["Zeus/Zeus.csproj", "Zeus/"]
RUN dotnet restore "Zeus/Zeus.csproj"
COPY . .
WORKDIR "/src/Zeus"
RUN dotnet build "Zeus.csproj" -c "${configuration}" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Zeus.csproj" -c "${configuration}" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zeus.dll"]