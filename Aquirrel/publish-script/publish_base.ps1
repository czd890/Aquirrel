#projectDir 项目所在文件夹名字
param($projectDir)
$orgDir = $MyInvocation.MyCommand.Definition|Split-Path -Parent
$projectRootDire = $orgDir |Split-Path -Parent
$nugetdownloadUrl = "https://nuget.org/nuget.exe"
$publishdir = $projectRootDire + "\.publish"
$nugetDir = $publishdir + "\.nuget"
$nuget = $nugetDir + "\nuget.exe"
$outputdir = $publishdir + "\" + $projectDir
write-Host "project dir: $projectDir"
write-Host "current dir: $currDir"
write-Host "nuget dir: $nugetDir"
write-Host "nuget file path: $nuget"
write-Host "nuget output dir: $outputdir"

function DownloadWithRetry([string] $url, [string] $downloadLocation, [int] $retries) {
    while ($true) {
        try {
            Invoke-WebRequest $url -OutFile $downloadLocation
            break
        }
        catch {
            $exceptionMessage = $_.Exception.Message
            Write-Host "Failed to download '$url': $exceptionMessage"
            if ($retries -gt 0) {
                $retries--
                Write-Host "Waiting 10 seconds before retrying. Retries left: $retries"
                Start-Sleep -Seconds 10

            }
            else {
                $exception = $_.Exception
                throw $exception
            }
        }
    }
}
if (!(Test-Path $publishdir)) {
    New-Item -Path $publishdir -ItemType Directory
}
if (!(Test-Path $nugetDir)) {
    New-Item -Path $nugetDir -ItemType Directory
}

if (!(Test-Path $nuget)) {
    write-Host "download nuget.exe"
    DownloadWithRetry -url $nugetdownloadUrl -downloadLocation $nuget -retries 3
}

if (!(Test-Path $outputdir)) {
    New-Item -Path $outputdir -Type directory | Out-Null
}



$rem = $outputdir + "\*"
Remove-Item $rem -recurse

cd $projectRootDire
cd $projectDir

dotnet pack -o $outputdir -c Release 
cd $nugetDir

$fileList = Get-ChildItem  -Path $outputdir -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -match "^((?!symbols).)*$" } | % {$_.FullName}
foreach ($f in $fileList) {
    write-Host "publish:::$f" 
    .\nuget.exe  push $f -ApiKey "25951454-f86d-475b-a314-9be39ed810d6"
}
cd $orgDir
write-Host "finsh"

