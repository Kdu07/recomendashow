# Build context = raiz do repo (InferenceService.csproj referencia ..\ml\AventuraML.csproj).
# Build local: docker build -f src/inference-service/Dockerfile -t aventura-recsys-inference:latest .

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

# Camada de restore separada: só invalida quando os .csproj mudam
COPY src/ml/AventuraML.csproj src/ml/
COPY src/inference-service/InferenceService.csproj src/inference-service/
RUN dotnet restore src/inference-service/InferenceService.csproj

COPY src/ml/ src/ml/
COPY src/inference-service/ src/inference-service/
RUN dotnet publish src/inference-service/InferenceService.csproj -c Release -o /out

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# libgomp1: exigida pelo binário nativo do MatrixFactorization (ML.NET)
# libfontconfig1: exigida pelo ScottPlot 5 (endpoints de gráficos)
# curl: usado pelo HEALTHCHECK
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgomp1 libfontconfig1 curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /out .

# data/models e data/dataset entram por bind mount em runtime (./data:/app/data:ro)
RUN useradd --create-home --shell /usr/sbin/nologin appuser
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# O endpoint responde 503 enquanto o HybridModelLoader carrega os artefatos e 200 quando pronto.
# start_period é a janela de carência do carregamento + primeiro batch; dimensionar com a medição
# da verificação local (~1,5–2x o tempo entre "Carregando modelo..." e "Modelo híbrido carregado").
HEALTHCHECK --start-period=180s --interval=30s --timeout=5s --retries=5 \
    CMD curl -f http://localhost:8080/api/diagnostics/health || exit 1

ENTRYPOINT ["dotnet", "InferenceService.dll"]
