# Use the official Ubuntu image as a parent image
FROM ubuntu:24.04 AS base
WORKDIR /app
EXPOSE 5000

# Install ASP.NET Core runtime from Ubuntu's repositories
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends aspnetcore-runtime-8.0 && \
    rm -rf /var/lib/apt/lists/*

# Use the SDK image to build the app
FROM ubuntu:24.04 AS build
WORKDIR /src

# Install .NET SDK from Ubuntu's repositories
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends dotnet-sdk-8.0 && \
    rm -rf /var/lib/apt/lists/*

# Copy csproj and restore dependencies
COPY ["src/UnitConversion.Api/UnitConversion.Api.csproj", "src/UnitConversion.Api/"]
COPY ["src/UnitConversion.Domain/UnitConversion.Domain.csproj", "src/UnitConversion.Domain/"]
RUN dotnet restore "src/UnitConversion.Api/UnitConversion.Api.csproj"

# Copy the remaining files and build
COPY . .
WORKDIR "/src/src/UnitConversion.Api"
RUN dotnet build "UnitConversion.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UnitConversion.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "UnitConversion.Api.dll"]
