using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aquirrel.EntityFramework.Test.Migrations
{
    public partial class hhh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelB",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    AId = table.Column<string>(maxLength: 32, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(maxLength: 22, nullable: true),
                    LastModfiyDate = table.Column<DateTime>(nullable: false),
                    TimeStamp = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelC",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    HH = table.Column<string>(maxLength: 32, nullable: true),
                    LastModfiyDate = table.Column<DateTime>(nullable: false),
                    TimeStamp = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelA",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    AId = table.Column<string>(maxLength: 32, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModfiyDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: true),
                    TimeStamp = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelA_ModelB_AId",
                        column: x => x.AId,
                        principalTable: "ModelB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelA_AId",
                table: "ModelA",
                column: "AId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelA");

            migrationBuilder.DropTable(
                name: "ModelC");

            migrationBuilder.DropTable(
                name: "ModelB");
        }
    }
}
