# --- Stage 1: Restore & Build ---
# Use the official .NET 8.0 SDK image to restore and build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["WeningerDemoProject.csproj", "./"]
RUN dotnet restore "WeningerDemoProject.csproj"

# Copy the entire source code and build the project in Release mode
COPY . .
RUN dotnet build "WeningerDemoProject.csproj" -c Release -o /app/build

# --- Stage 2: Publish ---
# Publish the application to a folder for deployment
FROM build AS publish
RUN dotnet publish "WeningerDemoProject.csproj" -c Release -o /app/publish

# --- Stage 3: Runtime ---
# Use the official .NET 8.0 ASP.NET runtime image for a smaller final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application (including the SQLite database file due to csproj settings)
COPY --from=publish /app/publish .

# Expose port 80 for HTTP traffic (default ASP.NET port)
EXPOSE 80

# Define the entry point to run the application
ENTRYPOINT ["dotnet", "WeningerDemoProject.dll"]
