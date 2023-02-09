FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ./src ./src

WORKDIR /src

RUN dotnet restore ./Scheme.Repl/Scheme.Repl.csproj
RUN dotnet restore ./Scheme.Tests/Scheme.Tests.csproj

RUN dotnet test ./Scheme.Tests/Scheme.Tests.csproj

RUN dotnet publish ./Scheme.Repl/Scheme.Repl.csproj -c Release -o /dist

FROM mcr.microsoft.com/dotnet/runtime:7.0
COPY --from=build /dist /app

WORKDIR /app
ENTRYPOINT ["dotnet", "Scheme.Repl.dll"]