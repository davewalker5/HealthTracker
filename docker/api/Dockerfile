FROM mcr.microsoft.com/dotnet/core/aspnet:latest
COPY healthtracker.api-1.40.0.0 /opt/healthtracker.api
WORKDIR /opt/healthtracker.api/bin
ENTRYPOINT [ "./HealthTracker.Api" ]
