using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShop1.Migrations
{
    /// <inheritdoc />
    public partial class New2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeminarOriginalName",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1L,
                column: "SeminarOriginalName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2L,
                column: "SeminarOriginalName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 3L,
                column: "SeminarOriginalName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 4L,
                column: "SeminarOriginalName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 5L,
                column: "SeminarOriginalName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Sandra", "Shandarovska", "1862022" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Risto", "Kizov", "842022" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Teodora", "Domazetovic", "1832023" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Kristina", "Kitrozoska", "852022" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Sara", "Atanasovska", "1672022" });

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AcademicRank", "FirstName", "LastName", "OfficeNumber" },
                values: new object[] { "Proffesor", "Slavica", "Pop-Stojanova", "A126" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeminarOriginalName",
                table: "Enrollments");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Marko", "Jovanov", "201001" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Elena", "Stojanovska", "201002" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Stefan", "Nikolov", "201003" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Ivana", "Trajkovska", "201004" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "FirstName", "LastName", "StudentId" },
                values: new object[] { "Bojan", "Ristov", "201005" });

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AcademicRank", "FirstName", "LastName", "OfficeNumber" },
                values: new object[] { "Professor", "Ivan", "Petrovski", "A101" });
        }
    }
}
