FROM microsoft/netcore:2.0- sdk as build

COPY . /src
WORKDIR /src/
RUN dotnet restore
WORKDIR /src/MatrixServer
RUN dotnet publish -c Release -o /publish

FROM microsoft/netcore:2.0-runtime
COPY --from=build /publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "MatrixServer.dll"]
