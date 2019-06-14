FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["FileProcessorService/FileProcessorService.csproj", "FileProcessorService/"]
RUN dotnet restore "FileProcessorService/FileProcessorService.csproj"
COPY . .
WORKDIR "/src/FileProcessorService"
RUN dotnet build "FileProcessorService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FileProcessorService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FileProcessorService.dll"]