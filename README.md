# Play.Catalog

Play Economy Catalog microservice.

## Create and publish package

```powershell
$version="1.0.3"
$owner="icodedotnetmicroservices"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Catalog.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Catalog -o ..\packages

dotnet nuget push ..\packages\Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image

```powershell
$env:GH_OWNER="icodedotnetmicroservices"
$env:GH_PAT="[PAT HERE]"

docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.catalog:$version .
```

## Run the docker image

```powershell
$cosmoDbConnString= "[CONN STRING HERE]"
$serviceBusConnString= "[CONN STRING HERE]"
docker run -it --rm -p 5000:5000 --name catalog -e MongoDbSettings__ConnectionString=$cosmoDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" play.catalog:$version
```
