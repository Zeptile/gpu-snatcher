# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY * /build/
WORKDIR /build/
RUN dotnet restore gpu-snatcher-worker.csproj

# copy everything else and build app
RUN dotnet publish gpu-snatcher-worker.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "gpu-snatcher-worker.dll"]
