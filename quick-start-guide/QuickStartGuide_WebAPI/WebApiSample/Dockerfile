# The .NET 10 images are based on Ubuntu 24.04.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Use the 'root' account to install fontconfig, which SkiaSharp needs; it brings FreeType and the DejaVu fonts too.
USER root
RUN apt-get update
RUN apt-get install -y --no-install-recommends libfontconfig1

# Switch back to the 'app' account to run the service.
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApiSample/WebApiSample.csproj", "WebApiSample/"]
RUN dotnet restore "./WebApiSample/WebApiSample.csproj"
COPY . .
WORKDIR "/src/WebApiSample"
RUN dotnet build "./WebApiSample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApiSample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiSample.dll"]