using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChoreCreatedById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Chores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Chores_CreatedById",
                table: "Chores",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Chores_Tenants_CreatedById",
                table: "Chores",
                column: "CreatedById",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chores_Tenants_CreatedById",
                table: "Chores");

            migrationBuilder.DropIndex(
                name: "IX_Chores_CreatedById",
                table: "Chores");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Chores");
        }
    }
}
