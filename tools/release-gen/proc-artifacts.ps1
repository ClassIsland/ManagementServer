$ErrorActionPreference = "Stop"
$artifacts = Get-ChildItem -Path ./out_artifacts -Directory


if ($(Test-Path ./out) -eq $false) {
    mkdir out
}

foreach ($artifact in $artifacts) {
    Copy-Item ./out_artifacts/$($artifact.Name)/* -Destination ./out/ -Recurse -Force
}
