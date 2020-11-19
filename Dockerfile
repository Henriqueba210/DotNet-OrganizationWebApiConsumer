FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

RUN mkdir -p /usr/share/man/man1 /usr/share/man/man2

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    openjdk-11-jre

RUN dotnet tool install --global dotnet-sonarscanner
ENV PATH="${PATH}:/root/.dotnet/tools"
ENV JAVA_TOOL_OPTIONS -Dfile.encoding=UTF8

COPY . ./OrganizationWebApiConsumer
WORKDIR /OrganizationWebApiConsumer

RUN dotnet restore

RUN dotnet test \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover

RUN dotnet sonarscanner begin /k:"OrganizationWebApiConsumer" \
    /d:sonar.host.url="http://192.168.0.106:9000" \
    /d:sonar.verbose=true \
    /v:1.0.0 \
    /d:sonar.login="160b1f8668ab12d00b8b917ae8f1d917565956d0" \
    /d:sonar.cs.opencover.reportsPaths="./Consumer.Tests/coverage.opencover.xml"

RUN dotnet build
RUN dotnet sonarscanner end /d:sonar.login="160b1f8668ab12d00b8b917ae8f1d917565956d0"
