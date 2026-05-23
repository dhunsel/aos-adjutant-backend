FROM node:24-bookworm-slim AS frontend-build

WORKDIR /frontend-app

COPY frontend/package.json frontend/package-lock.json ./

RUN npm ci

COPY frontend/ ./

RUN npm run build


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /backend-app

COPY backend/AosAdjutant.slnx ./
COPY backend/src/AosAdjutant.Api/AosAdjutant.Api.csproj src/AosAdjutant.Api/

RUN dotnet restore src/AosAdjutant.Api/AosAdjutant.Api.csproj

COPY backend/src/AosAdjutant.Api/ src/AosAdjutant.Api/
COPY backend/.editorconfig .editorconfig

COPY --from=frontend-build /frontend-app/dist/ src/AosAdjutant.Api/wwwroot/

RUN dotnet publish src/AosAdjutant.Api/AosAdjutant.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=backend-build /app/publish/ ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "AosAdjutant.Api.dll"]
