FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /newsreader

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./


# a full list of Runtime IDs https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
# RID for linux -r linux-x64
# No need for self-contained atrifact as IMAGE will have it already
RUN dotnet publish -c Release -o out 

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /newsreader
COPY --from=build-env /newsreader/out .
ENTRYPOINT ["dotnet", "NewsReaderSPA.dll"]
