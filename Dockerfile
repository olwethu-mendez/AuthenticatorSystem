# STAGE 1: Build & Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Step 1: Copy .csproj files and restore (to cache layers)
COPY ["AuthenticatorSystem/AuthenticatorSystem.csproj", "AuthenticatorSystem/"]
COPY ["BusinessLogicLayer/BusinessLogicLayer.csproj", "BusinessLogicLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
RUN dotnet restore "AuthenticatorSystem/AuthenticatorSystem.csproj"

# Step 2: Copy everything else and build
COPY . .
WORKDIR "/src/AuthenticatorSystem"
RUN dotnet build "AuthenticatorSystem.csproj" -c Release -o /app/build

# STAGE 2: Publish
FROM build AS publish
RUN dotnet publish "AuthenticatorSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 3: Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# DOD Requirement: Run on Port 8080 (Default for .NET 8)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthenticatorSystem.dll"]