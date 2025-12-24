using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkShop1.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AcquiredCredits", "CurrentSemester", "EducationLevel", "EnrollmentDate", "FirstName", "LastName", "StudentId" },
                values: new object[,]
                {
                    { 1L, null, 3, "Undergraduate", null, "Marko", "Jovanov", "201001" },
                    { 2L, null, 5, "Undergraduate", null, "Elena", "Petkovska", "201002" },
                    { 3L, null, 7, "Undergraduate", null, "Stefan", "Nikolov", "201003" },
                    { 4L, null, 1, "Undergraduate", null, "Ivana", "Trajkovska", "201004" },
                    { 5L, null, 6, "Undergraduate", null, "Bojan", "Ristov", "201005" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AcademicRank", "Degree", "FirstName", "HireDate", "LastName", "OfficeNumber" },
                values: new object[,]
                {
                    { 1, "Professor", "PhD", "Ivan", null, "Petrovski", "A101" },
                    { 2, "Associate Professor", "PhD", "Marija", null, "Stojanova", "A102" },
                    { 3, "Assistant", "MSc", "Petar", null, "Kostov", "B201" },
                    { 4, "Professor", "PhD", "Ana", null, "Dimitrova", "B202" },
                    { 5, "Assistant", "MSc", "Nikola", null, "Iliev", "C301" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Credits", "EducationLevel", "FirstTeacherId", "Programme", "SecondTeacherId", "Semester", "Title" },
                values: new object[,]
                {
                    { 1, 6, null, 1, "IT", 3, 3, "Databases" },
                    { 2, 6, null, 2, "IT", 4, 4, "Web Programming" },
                    { 3, 6, null, 1, "SE", 5, 5, "Software Engineering" },
                    { 4, 5, null, 4, "IT", 2, 6, "Computer Networks" },
                    { 5, 6, null, 3, "SE", 1, 5, "Operating Systems" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "AdditionalPoints", "CourseId", "ExamPoints", "FinishDate", "Grade", "ProjectPoints", "ProjectUrl", "Semester", "SeminarPoints", "SeminarUrl", "StudentId", "Year" },
                values: new object[,]
                {
                    { 1L, null, 1, null, null, 9, null, null, "Fall", null, null, 1L, 2024 },
                    { 2L, null, 1, null, null, 8, null, null, "Fall", null, null, 2L, 2024 },
                    { 3L, null, 2, null, null, 10, null, null, "Spring", null, null, 3L, 2024 },
                    { 4L, null, 3, null, null, 7, null, null, "Fall", null, null, 4L, 2024 },
                    { 5L, null, 4, null, null, 9, null, null, "Spring", null, null, 5L, 2024 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
