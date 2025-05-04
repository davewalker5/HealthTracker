FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
COPY healthtracker.mvc-1.23.0.0 /opt/healthtracker.mvc
WORKDIR /opt/healthtracker.mvc/bin
ENTRYPOINT [ "./HealthTracker.Mvc" ]
