using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeningerDemoProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNumber",
                table: "Orders");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerNumber",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
