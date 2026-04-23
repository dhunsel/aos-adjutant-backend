FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for restore layer caching
COPY AosAdjutant.slnx .
COPY src/AosAdjutant.Api/AosAdjutant.Api.csproj src/AosAdjutant.Api/

RUN dotnet restore src/AosAdjutant.Api/AosAdjutant.Api.csproj

COPY src/AosAdjutant.Api/ src/AosAdjutant.Api/
COPY .editorconfig .editorconfig

RUN dotnet publish src/AosAdjutant.Api/AosAdjutant.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AosAdjutant.Api.dll"]
