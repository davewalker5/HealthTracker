FROM mcr.microsoft.com/dotnet/core/aspnet:latest
ARG VERSION
COPY healthtracker.api-${VERSION}.0 /opt/healthtracker.api
WORKDIR /opt/healthtracker.api/bin
ENTRYPOINT [ "./HealthTracker.Api" ]
