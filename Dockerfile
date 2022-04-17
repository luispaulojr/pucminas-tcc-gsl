FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore

WORKDIR /src/app
RUN dotnet publish -c release -o /app --no-restore

RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https 
RUN dotnet dev-certs https --trust
RUN dotnet dev-certs https --check --verbose

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
# Use native linux file polling for better performance
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "app.dll"]