FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
# Install packages required for tesseract 3.3.0
RUN apt update && apt install libgif7 libjpeg62 libopenjp2-7 libpng16-16 libtiff5 libwebp6 libc6-dev libgdiplus -y && apt clean
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