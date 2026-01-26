# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore first for caching
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the app to a folder for runtime
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Expose the port your ASP.NET Core app listens on
EXPOSE 8080

# Let Kestrel listen on all interfaces
ENV ASPNETCORE_URLS=http://+:8080

# Copy published app from build stage
COPY --from=build /app/publish .

# Copy the .env file so your DotEnv class can read it
COPY .env .env

# Start the app
ENTRYPOINT ["dotnet", "Server.dll"]