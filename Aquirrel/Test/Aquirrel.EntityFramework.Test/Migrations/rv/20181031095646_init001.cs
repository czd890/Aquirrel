using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aquirrel.EntityFramework.Test.Migrations.rv
{
    public partial class init001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelASet",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModfiyDate = table.Column<DateTime>(nullable: false),
                    RowVersion = table.Column<int>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    StringDefault = table.Column<string>(nullable: true),
                    StringMax = table.Column<string>(nullable: true),
                    StringMaxLenAttr = table.Column<string>(maxLength: 999, nullable: true),
                    StringSetLength = table.Column<string>(maxLength: 640, nullable: true),
                    intDefault = table.Column<int>(nullable: false),
                    decimalDefault = table.Column<decimal>(nullable: false),
                    decimalSetSacle = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelASet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelASet");
        }
    }
}
