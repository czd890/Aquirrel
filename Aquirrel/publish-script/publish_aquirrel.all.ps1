param($releasenote, $fullversion)
Write-Host "releasenote:"$releasenote
Write-Host "fullversion:"$fullversion
if (([string]::IsNullOrWhiteSpace($releasenote)) -or ($releasenote -eq "?") -or ($releasenote -eq "/?") -or 
    ($releasenote -eq "--help") -or ($releasenote -eq "-help")) {
    Write-Host "-releasenote"
    Write-Host "-fullversion"
}
Set-Location ($MyInvocation.MyCommand.Definition|Split-Path -Parent)

.\publish_aquirrel -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.extensions $releasenote -fullversion $fullversion
##.\publish_aquirrel.autofac -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.entityframework -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.interceptor.castle -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.interceptor -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.logger.file -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.mq -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.resetapi -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.tracing -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.tools -releasenote $releasenote -fullversion $fullversion

Write-Host "all publish finished!!!"