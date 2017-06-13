param($releasenote,$fullversion)
.\publish_base.add.version -projectDir "Aquirrel.EntityFramework" -releasenote $releasenote -fullversion $fullversion
.\publish_base "Aquirrel.EntityFramework"