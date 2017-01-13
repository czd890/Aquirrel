param($releasenote,$fullversion)

.\publish_aquirrel -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.autofac -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.entityframework -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.interceptor.castle -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.interceptor -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.logger.file -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.mq -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.resetapi -releasenote $releasenote -fullversion $fullversion
.\publish_aquirrel.tracing -releasenote $releasenote -fullversion $fullversion

Write-Host "全部推送完毕"