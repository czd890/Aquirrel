using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

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
                    Id = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersion = table.Column<int>(type: "int", nullable: false),
                    StringDefault = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    StringMax = table.Column<string>(type: "longtext", nullable: true),
                    StringMaxLenAttr = table.Column<string>(type: "varchar(999)", maxLength: 999, nullable: true),
                    StringSetLength = table.Column<string>(type: "varchar(640)", maxLength: 640, nullable: true),
                    decimalDefault = table.Column<decimal>(type: "decimal(65, 30)", nullable: false),
                    decimalSetSacle = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    intDefault = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelASet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShardTable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DecimalSacle = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DefaultName = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MaxName = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    RowVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShardTable", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelASet");

            migrationBuilder.DropTable(
                name: "ShardTable");
        }
    }
}
