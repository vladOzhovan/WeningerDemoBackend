using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeningerDemoProject.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address_Apartment",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_HouseNumber",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Address_ZipCode",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Apartment",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_HouseNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customers");
        }
    }
}
