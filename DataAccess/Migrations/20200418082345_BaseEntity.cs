using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class BaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_InsertedUser",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "UpdatedUser",
                table: "Files",
                newName: "UpdateUser");

            migrationBuilder.RenameColumn(
                name: "InsertedUser",
                table: "Files",
                newName: "CreateUser");

            migrationBuilder.RenameColumn(
                name: "InsertDate",
                table: "Files",
                newName: "CreateDate");

            migrationBuilder.RenameIndex(
                name: "IX_Files_InsertedUser",
                table: "Files",
                newName: "IX_Files_CreateUser");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreateUser",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdateUser",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "UserOperationClaims",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreateUser",
                table: "UserOperationClaims",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "UserOperationClaims",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdateUser",
                table: "UserOperationClaims",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "OperationClaims",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreateUser",
                table: "OperationClaims",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "OperationClaims",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdateUser",
                table: "OperationClaims",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_CreateUser",
                table: "Files",
                column: "CreateUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_CreateUser",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateUser",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "UserOperationClaims");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "UserOperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "UserOperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdateUser",
                table: "UserOperationClaims");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdateUser",
                table: "OperationClaims");

            migrationBuilder.RenameColumn(
                name: "UpdateUser",
                table: "Files",
                newName: "UpdatedUser");

            migrationBuilder.RenameColumn(
                name: "CreateUser",
                table: "Files",
                newName: "InsertedUser");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Files",
                newName: "InsertDate");

            migrationBuilder.RenameIndex(
                name: "IX_Files_CreateUser",
                table: "Files",
                newName: "IX_Files_InsertedUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_InsertedUser",
                table: "Files",
                column: "InsertedUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
