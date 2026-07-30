FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
ARG VERSION
COPY healthtracker.mvc-${VERSION}.0 /opt/healthtracker.mvc
WORKDIR /opt/healthtracker.mvc/bin
ENTRYPOINT [ "./HealthTracker.Mvc" ]
