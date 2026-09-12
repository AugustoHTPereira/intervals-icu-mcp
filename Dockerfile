FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY IntervalsMcp.sln ./
COPY src/IntervalsIcu.Client/IntervalsIcu.Client.csproj src/IntervalsIcu.Client/
COPY src/IntervalsIcu.Mcp/IntervalsIcu.Mcp.csproj src/IntervalsIcu.Mcp/
RUN dotnet restore src/IntervalsIcu.Mcp/IntervalsIcu.Mcp.csproj

COPY src/ src/
RUN dotnet publish src/IntervalsIcu.Mcp/IntervalsIcu.Mcp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd --uid 5000 --create-home appuser
USER appuser

COPY --from=build /app ./

ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IntervalsIcu.Mcp.dll"]
