FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mini Weather Journal/Mini Weather Journal.csproj", "Mini Weather Journal/"]
RUN dotnet restore "Mini Weather Journal/Mini Weather Journal.csproj"
COPY . .
WORKDIR "/src/Mini Weather Journal"
RUN dotnet build "./Mini Weather Journal.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mini Weather Journal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mini Weather Journal.dll"]
