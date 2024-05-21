# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Set up the base image; this code is automatically built when the Dockerfile is created.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Use the 'root' account to update APT and the SkiaSharp-related dependencies.
USER root
RUN apt-get update
RUN apt-get install -y --no-install-recommends \
    libfontconfig1 libfreetype6 libgl1-mesa-dev \
    libglib2.0-0 libharfbuzz0b libjpeg62-turbo \
    libpng16-16 libx11-6 libxcb1 libxext6 libxrender1

# Use 'app' account to build, publish and finalize the image 
# these codes are automatically built when the Dockerfile is created. 
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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