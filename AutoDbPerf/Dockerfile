FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AutoDbPerf.csproj", "AutoDbPerf/"]
RUN dotnet restore "AutoDbPerf/AutoDbPerf.csproj" 

COPY . /src/AutoDbPerf
WORKDIR "/src/AutoDbPerf"
RUN dotnet build "AutoDbPerf.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoDbPerf.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update
RUN apt-get install -y postgresql-client	

RUN apt-get install -y unzip
COPY clickhouse.zip .
RUN unzip clickhouse.zip clickhouse
RUN ./clickhouse install
ENV HOST="host.docker.internal"
ENV QUERYPATH="/app/queries"
ENTRYPOINT ["dotnet", "AutoDbPerf.dll"]
