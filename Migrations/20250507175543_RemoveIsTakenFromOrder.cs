using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeningerDemoProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsTakenFromOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTaken",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaken",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
