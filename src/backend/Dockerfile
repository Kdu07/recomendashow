FROM mcr.microsoft.com/dotnet/sdk:9.0.6 AS build
WORKDIR /src

# Copia só o csproj primeiro (ativa cache de restore)
COPY *.csproj ./
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:9.0.6 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app ./

RUN useradd --no-log-init --create-home --shell /usr/sbin/nologin appuser
USER appuser

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5253
EXPOSE 5253

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD curl -f http://localhost:5253/health || exit 1

ENTRYPOINT ["dotnet", "BackEnd.Aventura.dll"]
