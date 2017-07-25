using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Aquirrel.EntityFramework.Test.Migrations
{
    public partial class init : Migration
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
                    StringMax = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    StringSetLength = table.Column<string>(type: "nvarchar(640)", maxLength: 640, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    decimalDefault = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    decimalSetSacle = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    intDefault = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelASet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Table_attr_name",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModfiyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
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
                name: "Table_attr_name");
        }
    }
}
