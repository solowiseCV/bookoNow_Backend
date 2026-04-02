FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first (for layer caching)
COPY BookNow.Domain/BookNow.Domain.csproj BookNow.Domain/
COPY BookNow.Application/BookNow.Application.csproj BookNow.Application/
COPY BookNow.Infrastructure/BookNow.Infrastructure.csproj BookNow.Infrastructure/
COPY BookNow.Presentation/BookNow.Presentation.csproj BookNow.Presentation/

RUN dotnet restore BookNow.Presentation/BookNow.Presentation.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish BookNow.Presentation/BookNow.Presentation.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BookNow.Presentation.dll"]
