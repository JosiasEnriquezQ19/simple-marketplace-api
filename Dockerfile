# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SimpleMarketplace.Api.csproj", "./"]
RUN dotnet restore "SimpleMarketplace.Api.csproj"
COPY . .
RUN dotnet build "SimpleMarketplace.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleMarketplace.Api.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SimpleMarketplace.Api.dll"]
