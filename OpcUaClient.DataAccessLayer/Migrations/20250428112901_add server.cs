using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpcUaClient.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addserver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServerId",
                table: "Tag",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "text", nullable: false),
                    ServerAddress = table.Column<string>(type: "text", nullable: false),
                    ServerPortNumber = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ServerId",
                table: "Tag",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Servers_ServerId",
                table: "Tag",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Servers_ServerId",
                table: "Tag");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Tag_ServerId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Tag");
        }
    }
}
