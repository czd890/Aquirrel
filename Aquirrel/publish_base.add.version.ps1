#fullversion 全版本替换
#level 需要修改的版本级别：1，2，3，4
#levelversion 需要修改的版本级别替换的值
#projectDir 指定项目，不指定项目则搜索当前目录下所有项目
#releasenote 更新说明
param($fullversion,$level,$levelversion,$projectDir,$releasenote)

$OutputEncoding = New-Object -typename System.Text.UTF8Encoding

$currDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$orgDir=$currDir
write-Host "fullversion: $fullversion"
write-Host "level: $level"
write-Host "levelversion: $levelversion"
write-Host "projectDir: $projectDir"
write-Host "releaseNotes: $releasenote"
write-Host "currDir: $currDir"
function SetVer ($ver,$level,$value) {
	write-Host "ver:"+$ver+",level:"+$level+",value:"+$value
	$verArr= $ver.Split('.')

	if($level -lt 0 -or $level -gt $verArr.Length){
		$level=$verArr.Length
	}

	if(![string]::IsNullOrEmpty($value)){
		$verArr[$level-1]=$value
	}else{
		 $v=0
		if([int]::TryParse($verArr[$level-1], [ref]$v)){
			$v+=1
		}else{
			$v= [int]([datetime]::Now - [datetime]::Parse("2017/1/1")).TotalMinutes
		}
		$verArr[$level-1]=$v
	}

	$s=""
	for ($i = 0; $i -lt $verArr.Length; $i++) {
		$s+= $verArr[$i]+"."
	}
	$s=$s.Substring(0,$s.Length-1)
	return $s
}
try {
	if (![string]::IsNullOrEmpty($projectDir)) {
		write-Host "cd $projectDir"
		Set-Location $projectDir
		$currDir=Get-Location
		write-Host "cd after: "+$currDir
	} 

	$jsonPath=[System.IO.Path]::Combine($currDir,"project.json")
	$jsonContent=[System.IO.File]::ReadAllText($jsonPath);
	$json=ConvertFrom-Json -InputObject $jsonContent;

	# $jsonContent | Out-Host

	if (![string]::IsNullOrEmpty($releasenote)) {
		$json.packOptions.releaseNotes=$releasenote
	}



	$currVer=$json.version
	if (![string]::IsNullOrEmpty($fullversion)) {
		$currVer=$fullversion
	}else{
		if([string]::IsNullOrEmpty($level))
		{
			$level=(-1)
		}
		$currVer=SetVer $currVer $level $levelversion 
	}

	write-Host "currVer: "+$currVer
	$json.version=$currVer

	$jsonContent2 =ConvertTo-Json -InputObject $json
}
catch   {
	Write-Host $_
	Set-Location $orgDir
	return
}

#write-Host "jsonContent: "+$jsonContent2

#$jsonPath=[System.IO.Path]::Combine($currDir,"project_new.json")

[System.IO.File]::WriteAllText($jsonPath,$jsonContent2,[System.Text.Encoding]::UTF8)

Set-Location $orgDir