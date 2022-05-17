# Play.Catalog

Play Economy Catalog microservice.

## Create and publish package

```powershell
$version="1.0.4"
$owner="icodedotnetmicroservices"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Catalog.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Catalog -o ..\packages

dotnet nuget push ..\packages\Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image

```powershell
$env:GH_OWNER="icodedotnetmicroservices"
$env:GH_PAT="[PAT HERE]"
$containerregisteryname = "acrplayeconomy"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$containerregisteryname.azurecr.io/play.catalog:$version" .
```

## Run the docker image

```powershell
$cosmoDbConnString= "[CONN STRING HERE]"
$serviceBusConnString= "[CONN STRING HERE]"
docker run -it --rm -p 5000:5000 --name catalog -e MongoDbSettings__ConnectionString=$cosmoDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" play.catalog:$version
```

## Publishing The Docker Image

```powershell
az acr login --name $containerregisteryname
docker push "$containerregisteryname.azurecr.io/play.catalog:$version"
```

## Creating The Pod Managed Identity

````powershell
$appname = "playeconomy"
$aksclustername = "aksclusterplayeconomy"
$namespace="catalog"
az identity create --resource-group $appname --name $namespace
$IDENTITY_RESOURCE_ID=az identity show -g $appname -n $namespace --query id -otsv

az aks pod-identity add --resource-group $appname --cluster-name $aksclustername --namespace $namespace --name $namespace --identity-resource-id $IDENTITY_RESOURCE_ID

## Granting access to Key Vault Secrets

```powershell
$azurekeyvaultname = "azkeyvaultplayeconomy"
$IDENTITY_CLIENT_ID=az identity show -g $appname -n $namespace --query clientId -otsv
az keyvault set-policy -n $azurekeyvaultname --secret-permissions  get list --spn $IDENTITY_CLIENT_ID

````
