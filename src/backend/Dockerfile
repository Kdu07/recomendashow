FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copia só o csproj primeiro (isso ativa cache de restore)
COPY *.csproj ./
RUN dotnet restore

# 2. Agora copia o resto do projeto
COPY . .

# 3. Build leve (sem restore de novo)
RUN dotnet publish -c Release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:5253
EXPOSE 5253

ENTRYPOINT ["dotnet", "BackEnd.Aventura.dll"]