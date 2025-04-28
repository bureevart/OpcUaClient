using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpcUaClient.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatetag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ConvertBeforeRecalcForRead",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ConvertBeforeRecalcForWrite",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "HasWriteRegister",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Offset",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "OutputType",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Recalc",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "RoundingAccuracy",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UseOutputType",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "WriteRegisterAddress",
                table: "Tag");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tag",
                newName: "StatusCode");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Tag",
                newName: "NodeId");

            migrationBuilder.AddColumn<string>(
                name: "CurrentValue",
                table: "Tag",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Tag",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastGoodValue",
                table: "Tag",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSourceTimeStamp",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedTime",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentValue",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "LastGoodValue",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "LastSourceTimeStamp",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "LastUpdatedTime",
                table: "Tag");

            migrationBuilder.RenameColumn(
                name: "StatusCode",
                table: "Tag",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "Tag",
                newName: "Comment");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Address",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ConvertBeforeRecalcForRead",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConvertBeforeRecalcForWrite",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "Factor",
                table: "Tag",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "HasWriteRegister",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "Offset",
                table: "Tag",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "OutputType",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Recalc",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "RoundingAccuracy",
                table: "Tag",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UseOutputType",
                table: "Tag",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WriteRegisterAddress",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
