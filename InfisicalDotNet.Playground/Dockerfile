FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InfisicalDotNet.Playground/InfisicalDotNet.Playground.csproj", "InfisicalDotNet.Playground/"]
RUN dotnet restore "InfisicalDotNet.Playground/InfisicalDotNet.Playground.csproj"
COPY . .
WORKDIR "/src/InfisicalDotNet.Playground"
RUN dotnet build "InfisicalDotNet.Playground.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InfisicalDotNet.Playground.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfisicalDotNet.Playground.dll"]
