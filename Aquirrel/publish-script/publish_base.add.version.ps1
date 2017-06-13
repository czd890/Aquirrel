#fullversion 全版本替换
#level 需要修改的版本级别：1，2，3，4
#levelversion 需要修改的版本级别替换的值
#projectDir 项目所在文件夹名字
#releasenote 更新说明
param($fullversion, $level, $levelversion, $projectDir, $releasenote)

$currDir = $MyInvocation.MyCommand.Definition|Split-Path -Parent
$orgDir = $currDir

write-Host "fullversion: $fullversion"
write-Host "level: $level"
write-Host "levelversion: $levelversion"
write-Host "projectDir: $projectDir"
write-Host "releaseNotes: $releasenote"
write-Host "currDir: $currDir"
function SetVer ($ver, $level, $value) {
    write-Host "ver:"$ver",level:"$level+",value:"$value
    $verArr = $ver.Split('.')

    if ($level -lt 0 -or $level -gt $verArr.Length) {
        $level = $verArr.Length
    }

    if (![string]::IsNullOrEmpty($value)) {
        $verArr[$level - 1] = $value
    }
    else {
        $v = 0
        if ([int]::TryParse($verArr[$level - 1], [ref]$v)) {
            $v += 1
        }
        else {
            $v = [int]([datetime]::Now - [datetime]::Parse("2017/1/1")).TotalMinutes
        }
        $verArr[$level - 1] = $v
    }

    $s = ""
    for ($i = 0; $i -lt $verArr.Length; $i++) {
        $s += $verArr[$i] + "."
    }
    $s = $s.Substring(0, $s.Length - 1)
    return $s
}
try {
    if (![string]::IsNullOrEmpty($projectDir)) {
        write-Host "cd ../$projectDir"
        Set-Location ../$projectDir
        $currDir = Get-Location
        write-Host "cd after: " $currDir
    } 
   
    $projectFile = Get-ChildItem -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -match ".*\.csproj$"} | Select-Object -First 1 | Select-Object -ExpandProperty "Name"
    write-Host $projectFile
    
    $projectXml = [xml](Get-Content $projectFile -Encoding "utf8")

    if (![string]::IsNullOrEmpty($releasenote)) {
        write-Host "release notes: "$projectXml.Project.PropertyGroup.PackageReleaseNotes" -->" $releasenote
        $projectXml.Project.PropertyGroup.PackageReleaseNotes = $releasenote
    }
    $currVer = $projectXml.Project.PropertyGroup.VersionPrefix
    if (![string]::IsNullOrEmpty($fullversion)) {
        $currVer = $fullversion
    }
    else {
        if ([string]::IsNullOrEmpty($level)) {
            $level = (-1)
        }
        $currVer = SetVer $currVer $level $levelversion 
    }

    write-Host "version: "$projectXml.Project.PropertyGroup.VersionPrefix" -->"$currVer
    $projectXml.Project.PropertyGroup.VersionPrefix = $currVer
}catch {
    Write-Host $_
    Set-Location $orgDir
    return
}
$projectXml.save("$currDir\$projectFile")
Set-Location $orgDir