using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpcUaClient.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Address = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Recalc = table.Column<bool>(type: "boolean", nullable: false),
                    Factor = table.Column<float>(type: "real", nullable: false),
                    Offset = table.Column<float>(type: "real", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    HasWriteRegister = table.Column<bool>(type: "boolean", nullable: false),
                    WriteRegisterAddress = table.Column<int>(type: "integer", nullable: false),
                    OutputType = table.Column<int>(type: "integer", nullable: false),
                    UseOutputType = table.Column<bool>(type: "boolean", nullable: false),
                    RoundingAccuracy = table.Column<byte>(type: "smallint", nullable: false),
                    ConvertBeforeRecalcForRead = table.Column<bool>(type: "boolean", nullable: false),
                    ConvertBeforeRecalcForWrite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
