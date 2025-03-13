FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore

WORKDIR /app
COPY FirstFunction/*.csproj ./FirstFunction/
COPY Tests/*.csproj ./Tests/
COPY *.sln ./
RUN dotnet restore

FROM restore as build
WORKDIR /app
COPY . .
COPY --from=restore /app ./
RUN dotnet build --no-restore -c Release

FROM build AS test
WORKDIR /app
COPY --from=build /app ./
RUN dotnet test -c Release --no-build

FROM build AS publish
WORKDIR /app
# Publish the main project (e.g., Proj) to a /publish folder
RUN dotnet publish FirstFunction/first-function.csproj -c Release -o publish --no-build

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=publish ["/app/publish", "/home/site/wwwroot"]