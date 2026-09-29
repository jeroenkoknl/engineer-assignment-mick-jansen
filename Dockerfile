FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/SwapfietsApi/*.csproj ./
RUN dotnet restore

COPY src/SwapfietsApi/. ./
RUN dotnet publish -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV API_KEY=sk-swapfiets-prod-9f3a2c

EXPOSE 80

ENTRYPOINT ["dotnet", "SwapfietsApi.dll"]
