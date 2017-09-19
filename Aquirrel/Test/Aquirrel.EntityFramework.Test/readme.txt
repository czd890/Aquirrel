add-migration init001 -Context LogDbContext -OutputDir "Migrations/log" -StartupProject "Aquirrel.EntityFramework.Test" -Project "Aquirrel.EntityFramework.Test"


add-migration init001 -Context TestDbContext -OutputDir "Migrations/test" -StartupProject "Aquirrel.EntityFramework.Test" -Project "Aquirrel.EntityFramework.Test"

add-migration init001 -Context RVDbContext -OutputDir "Migrations/rv" -StartupProject "Aquirrel.EntityFramework.Test" -Project "Aquirrel.EntityFramework.Test"


update-database -Migration init001 -Context LogDbContext -StartupProject "Aquirrel.EntityFramework.Test" -Project "Aquirrel.EntityFramework.Test" -verbose



update-database -Context RVDbContext -StartupProject "Aquirrel.EntityFramework.Test" -Project "Aquirrel.EntityFramework.Test" -verbose