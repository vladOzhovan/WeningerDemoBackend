# --- Stage 1: Restore & Build ---
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["WeningerDemoProject.csproj", "./"]
RUN dotnet restore "WeningerDemoProject.csproj"

# Copy all source code and build the project
COPY . .
RUN dotnet build "WeningerDemoProject.csproj" -c Release -o /app/build

# --- Stage 2: Publish ---
FROM build AS publish

# Publish the app into a folder for deployment
RUN dotnet publish "WeningerDemoProject.csproj" -c Release -o /app/publish

# --- Stage 3: Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copy published output from the previous stage
COPY --from=publish /app/publish .

# Define the entry point for the container
ENTRYPOINT ["dotnet", "WeningerDemoProject.dll"]
