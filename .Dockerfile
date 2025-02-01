FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR .
COPY ["MyDigitalWallet.sln", "."]
COPY ["MyDigitalWallet.API/MyDigitalWallet.API.csproj", "MyDigitalWallet.API/"]
COPY ["MyDigitalWallet.Application/MyDigitalWallet.Application.csproj", "MyDigitalWallet.Application/"]
COPY ["MyDigitalWallet.Domain/MyDigitalWallet.Domain.csproj", "MyDigitalWallet.Domain/"]
COPY ["MyDigitalWallet.Infrastructure/MyDigitalWallet.Infrastructure.csproj", "MyDigitalWallet.Infrastructure/"]

RUN dotnet restore "MyDigitalWallet.API/MyDigitalWallet.API.csproj"
COPY . .
WORKDIR "/MyDigitalWallet.API"
RUN dotnet build "MyDigitalWallet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyDigitalWallet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyDigitalWallet.API.dll"]
