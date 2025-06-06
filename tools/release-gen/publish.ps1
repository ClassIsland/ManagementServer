param($os, $arch)

$ErrorActionPreference = "Stop"

$PUBLISH_TARGET = "..\out\ClassIsland.ManagementServer"

if ($(Test-Path ./out) -eq $false) {
    mkdir out
} else {
    rm out/* -Recurse -Force
}
#dotnet clean


Write-Host "Publish parameters: OS=$os, Platform=$arch"

if (($os -eq "any" ) -and ($arch -eq "any")) {
    $runtimeIdentifier = "-p:RuntimeIdentifier="
} else {
    $runtimeIdentifier = "-p:RuntimeIdentifier=$os-$arch"
}

# PNPM Install
Write-Host "Installing Node Denpendencies..." -ForegroundColor Cyan
cd classisland.managementserver.client
pnpm install
cd ..

Write-Host "Building CIMS..." -ForegroundColor Cyan
dotnet publish .\ClassIsland.ManagementServer.Server\ClassIsland.ManagementServer.Server.csproj -c Release -p:PublishDir=$PUBLISH_TARGET $runtimeIdentifier

Write-Host "Packaging..." -ForegroundColor Cyan

rm ./out/ClassIsland/*.xml -ErrorAction Continue
7z a ./out/${env:artifact_name}.zip ./out/ClassIsland.ManagementServer/* -r

Write-Host "Successfully published to $PUBLISH_TARGET" -ForegroundColor Green