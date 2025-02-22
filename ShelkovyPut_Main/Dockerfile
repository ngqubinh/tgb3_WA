# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app 
EXPOSE 80

# Install necessary libraries
RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2

# Use the official .NET 8 SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files
COPY ["ShelkovyPut_Main/ShelkovyPut_Main.csproj", "ShelkovyPut_Main/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ShelkovyPut_Main/ShelkovyPut_Main.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ShelkovyPut_Main"
RUN dotnet build "ShelkovyPut_Main.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "ShelkovyPut_Main.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "ShelkovyPut_Main.dll"]
