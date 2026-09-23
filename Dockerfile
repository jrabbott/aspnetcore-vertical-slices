# Runtime-only image. Build context must be a pre-published app folder
# (e.g. ./.publish/web from: dotnet publish … -o ./.publish/web).
# Do not restore/build/publish inside this image — build once on the host/CI.
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID . .

USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WeatherApp.dll"]
