using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShop1.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                column: "LastName",
                value: "Stojanovska");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                column: "LastName",
                value: "Petkovska");
        }
    }
}
