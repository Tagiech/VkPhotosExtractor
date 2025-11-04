FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5105

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VkPhotosExtractor.Web/VkPhotosExtractor.Web.csproj", "VkPhotosExtractor.Web/"]
COPY ["VkPhotosExtractor.Application/VkPhotosExtractor.Application.csproj", "VkPhotosExtractor.Application/"]
COPY ["VkPhotosExtractor.Integration/VkPhotosExtractor.Integration.csproj", "VkPhotosExtractor.Integration/"]
COPY ["VkPhotosExtractor.Cache/VkPhotosExtractor.Cache.csproj", "VkPhotosExtractor.Cache/"]
RUN dotnet restore "VkPhotosExtractor.Web/VkPhotosExtractor.Web.csproj"
COPY . .
WORKDIR "/src/VkPhotosExtractor.Web"
RUN dotnet build "./VkPhotosExtractor.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VkPhotosExtractor.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VkPhotosExtractor.Web.dll"]
