# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar o restante e compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Porta padrão usada pelo Render
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "UserAuthApi.dll"]
