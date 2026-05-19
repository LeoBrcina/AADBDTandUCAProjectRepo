# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first
COPY PicGram.sln ./
COPY PicGramWebApp/PicGramWebApp.csproj PicGramWebApp/

# Restore dependencies
RUN dotnet restore PicGramWebApp/PicGramWebApp.csproj

# Copy the rest of the source code
COPY . .

# Publish the application
WORKDIR /src/PicGramWebApp
RUN dotnet publish PicGramWebApp.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# ASP.NET Core port
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "PicGramWebApp.dll"]