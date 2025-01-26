param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location,
    
    [Parameter(Mandatory=$true)]
    [string]$UnsplashAccessKey
)

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Ensure Azure CLI is logged in
$account = az account show | ConvertFrom-Json
if (-not $account) {
    Write-Host "Please log in to Azure first using 'az login'"
    exit 1
}

# Create resource group if it doesn't exist
$rg = az group show --name $ResourceGroupName 2>$null | ConvertFrom-Json
if (-not $rg) {
    Write-Host "Creating resource group $ResourceGroupName..."
    az group create --name $ResourceGroupName --location $Location
}

# Update parameters file with Unsplash key
$parametersFile = Join-Path $scriptDir "parameters.json"
$parameters = Get-Content $parametersFile -ErrorAction Stop | ConvertFrom-Json
$parameters.parameters.unsplashAccessKey.value = $UnsplashAccessKey
$parameters | ConvertTo-Json -Depth 10 | Set-Content $parametersFile

# Build and publish the function app
Write-Host "Building and publishing the function app..."
$solutionDir = Split-Path -Parent $scriptDir
$publishPath = Join-Path $scriptDir "publish"
dotnet publish (Join-Path $solutionDir "WeatherStation.ImageProcessor.Functions/WeatherStation.ImageProcessor.Functions.csproj") -c Release -o $publishPath

# Deploy Azure resources using Bicep
Write-Host "Deploying Azure resources..."
$bicepFile = Join-Path $scriptDir "main.bicep"
$deployment = az deployment group create `
    --resource-group $ResourceGroupName `
    --template-file $bicepFile `
    --parameters $parametersFile | ConvertFrom-Json

if (-not $deployment) {
    Write-Error "Deployment failed!"
    exit 1
}

# Get function app name from deployment output
$functionAppName = $deployment.properties.outputs.functionAppName.value
if (-not $functionAppName) {
    Write-Error "Failed to get function app name from deployment output"
    exit 1
}

Write-Host "Deploying function app code to $functionAppName..."
$zipPath = Join-Path $scriptDir "publish.zip"
Compress-Archive -Path "$publishPath/*" -DestinationPath $zipPath -Force

az functionapp deployment source config-zip `
    --resource-group $ResourceGroupName `
    --name $functionAppName `
    --src $zipPath

# Clean up
Remove-Item $zipPath -Force -ErrorAction SilentlyContinue
Remove-Item $publishPath -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Deployment complete!"
Write-Host "Function App URL: $($deployment.properties.outputs.functionAppUrl.value)"