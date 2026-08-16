# デバッグ コンテナーをカスタマイズする方法と、Visual Studio がこの Dockerfile を使用してより高速なデバッグのためにイメージをビルドする方法については、https://aka.ms/customizecontainer をご覧ください。

# このステージは、VS から高速モードで実行するときに使用されます (デバッグ構成の既定値)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# このステージは、サービス プロジェクトのビルドに使用されます
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TwitchStreamTools/TwitchStreamTools.csproj", "TwitchStreamTools/"]
RUN dotnet restore "./TwitchStreamTools/TwitchStreamTools.csproj"
COPY . .
WORKDIR "/src/TwitchStreamTools"
RUN dotnet build "./TwitchStreamTools.csproj" -c $BUILD_CONFIGURATION -o /app/build

# このステージは、最終ステージにコピーするサービス プロジェクトを公開するために使用されます
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TwitchStreamTools.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Deno binary
FROM denoland/deno:bin AS deno

# このステージは、運用環境または VS から通常モードで実行している場合に使用されます (デバッグ構成を使用しない場合の既定)
FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y ffmpeg

COPY --from=deno /deno /usr/local/bin/deno
RUN chmod +x /usr/local/bin/deno

RUN mkdir -p /data /Downloads && chown -R $APP_UID:$APP_UID /data /Downloads
VOLUME /data /Downloads

COPY --from=publish /app/publish .

RUN chown -R $APP_UID:$APP_UID /app

USER $APP_UID
ENTRYPOINT ["dotnet", "TwitchStreamTools.dll"]