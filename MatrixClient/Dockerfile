FROM microsoft/netcore:2.0- sdk as build

COPY . /src
WORKDIR /src/
RUN dotnet restore
WORKDIR /src/MatrixClient
RUN dotnet publish -c Release -o /publish

FROM microsoft/netcore:2.0-runtime
COPY --from=build /publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "MatrixClient.dll"]
