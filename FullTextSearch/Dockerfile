FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["FullTextSearch/FullTextSearch.csproj", "FullTextSearch/"]
RUN dotnet restore "FullTextSearch/FullTextSearch.csproj"
COPY . .
WORKDIR "/src/FullTextSearch"
RUN dotnet build "FullTextSearch.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FullTextSearch.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FullTextSearch.dll"]
