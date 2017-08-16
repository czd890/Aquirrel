using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Aquirrel.EntityFramework.Test.Migrations.test
{
    public partial class init001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelASet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StringDefault = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    StringMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringMaxLenAttr = table.Column<string>(type: "nvarchar(999)", maxLength: 999, nullable: true),
                    StringSetLength = table.Column<string>(type: "nvarchar(640)", maxLength: 640, nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    decimalDefault = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
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
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecimalSacle = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DefaultName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShardTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Table_attr_name",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table_attr_name", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelASet");

            migrationBuilder.DropTable(
                name: "ShardTable");

            migrationBuilder.DropTable(
                name: "Table_attr_name");
        }
    }
}
