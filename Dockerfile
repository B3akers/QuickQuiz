# ===========================================
# Stage 1: Build .NET API
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-build
WORKDIR /src
COPY QuickQuiz.API/*.csproj ./QuickQuiz.API/
RUN dotnet restore QuickQuiz.API/QuickQuiz.API.csproj
COPY QuickQuiz.API/ ./QuickQuiz.API/
RUN dotnet publish QuickQuiz.API/QuickQuiz.API.csproj -c Release -o /app

# ===========================================
# Stage 2: Build SvelteKit Client
# ===========================================
FROM node:20-alpine AS client-build
WORKDIR /app
COPY QuickQuiz.Client/package*.json ./
RUN npm ci
COPY QuickQuiz.Client/ ./
RUN npm run build

# ===========================================
# Stage 3: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=api-build /app .
COPY --from=client-build /app/build ./wwwroot

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "QuickQuiz.API.dll"]
