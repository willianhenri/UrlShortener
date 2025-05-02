# Estágio 1: Build
# Use a imagem oficial do SDK .NET 8 - Escolha uma versão estável específica
FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build
WORKDIR /source

# Copia o csproj e restaura pacotes (cache otimizado)
COPY *.csproj .
RUN dotnet restore --use-current-runtime

# Copia o resto do código e publica a aplicação
COPY . .
# O --use-current-runtime pode ajudar a evitar problemas de TargetFramework/RuntimeIdentifier
RUN dotnet publish -c Release -o /app/out --no-restore --use-current-runtime

# Estágio 2: Runtime
# Use a imagem oficial de runtime do ASP.NET Core - Combine a versão major.minor
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Define a porta que a aplicação vai ouvir DENTRO do container
# Kestrel geralmente usa a porta 8080 por padrão em containers .NET 8+
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]