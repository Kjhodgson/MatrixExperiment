FROM microsoft/dotnet:2.0-sdk as build

COPY . /src
WORKDIR /src
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

FROM microsoft/dotnet:2.0-runtime
COPY --from=build /publish /app
WORKDIR /app
EXPOSE 8002
ENTRYPOINT ["dotnet", "MatrixServer.dll"]