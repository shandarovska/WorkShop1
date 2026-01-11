using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;
using WorkShop1.Models;

namespace WorkShop1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course → FirstTeacher
            modelBuilder.Entity<Course>()
                .HasOne(c => c.FirstTeacher)
                .WithMany(t => t.FirstTeacherCourses) //eden nastavnik povekje predmeti
                .HasForeignKey(c => c.FirstTeacherId) 
                .OnDelete(DeleteBehavior.Restrict);

            // Course → SecondTeacher
            modelBuilder.Entity<Course>()
                .HasOne(c => c.SecondTeacher)
                .WithMany(t => t.SecondTeacherCourses)
                .HasForeignKey(c => c.SecondTeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment → Course
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course) //enrollments ima eden course
                .WithMany(c => c.Enrollments) // course povekje enrollments
                .HasForeignKey(e => e.CourseId);

            // Enrollment → Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student) //enrollment ima eden student
                .WithMany(s => s.Enrollments) //eden student ima povekje enrollments
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<ApplicationUser>() // POVRZUVAME
                .HasOne(u => u.Student)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Teacher)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // teachers
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, FirstName = "Slavica", LastName = "Pop-Stojanova", Degree = "PhD", AcademicRank = "Proffesor", OfficeNumber = "A126" },
                new Teacher { Id = 2, FirstName = "Marija", LastName = "Stojanova", Degree = "PhD", AcademicRank = "Associate Professor", OfficeNumber = "A102" },
                new Teacher { Id = 3, FirstName = "Petar", LastName = "Kostov", Degree = "MSc", AcademicRank = "Assistant", OfficeNumber = "B201" },
                new Teacher { Id = 4, FirstName = "Ana", LastName = "Dimitrova", Degree = "PhD", AcademicRank = "Professor", OfficeNumber = "B202" },
                new Teacher { Id = 5, FirstName = "Nikola", LastName = "Iliev", Degree = "MSc", AcademicRank = "Assistant", OfficeNumber = "C301" }
            );

            // students
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, StudentId = "1862022", FirstName = "Sandra", LastName = "Shandarovska", CurrentSemester = 3, EducationLevel = "Undergraduate" },
                new Student { Id = 2, StudentId = "842022", FirstName = "Risto", LastName = "Kizov", CurrentSemester = 5, EducationLevel = "Undergraduate" },
                new Student { Id = 3, StudentId = "1832023", FirstName = "Teodora", LastName = "Domazetovic", CurrentSemester = 7, EducationLevel = "Undergraduate" },
                new Student { Id = 4, StudentId = "852022", FirstName = "Kristina", LastName = "Kitrozoska", CurrentSemester = 1, EducationLevel = "Undergraduate" },
                new Student { Id = 5, StudentId = "1672022", FirstName = "Sara", LastName = "Atanasovska", CurrentSemester = 6, EducationLevel = "Undergraduate" }
            );

            // courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "Databases", Credits = 6, Semester = 3, Programme = "IT", FirstTeacherId = 1, SecondTeacherId = 3 },
                new Course { Id = 2, Title = "Web Programming", Credits = 6, Semester = 4, Programme = "IT", FirstTeacherId = 2, SecondTeacherId = 4 },
                new Course { Id = 3, Title = "Software Engineering", Credits = 6, Semester = 5, Programme = "SE", FirstTeacherId = 1, SecondTeacherId = 5 },
                new Course { Id = 4, Title = "Computer Networks", Credits = 5, Semester = 6, Programme = "IT", FirstTeacherId = 4, SecondTeacherId = 2 },
                new Course { Id = 5, Title = "Operating Systems", Credits = 6, Semester = 5, Programme = "SE", FirstTeacherId = 3, SecondTeacherId = 1 }
            );

            // enrollments
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { Id = 1, CourseId = 1, StudentId = 1, Semester = "Fall", Year = 2024, Grade = 9 },
                new Enrollment { Id = 2, CourseId = 1, StudentId = 2, Semester = "Fall", Year = 2024, Grade = 8 },
                new Enrollment { Id = 3, CourseId = 2, StudentId = 3, Semester = "Spring", Year = 2024, Grade = 10 },
                new Enrollment { Id = 4, CourseId = 3, StudentId = 4, Semester = "Fall", Year = 2024, Grade = 7 },
                new Enrollment { Id = 5, CourseId = 4, StudentId = 5, Semester = "Spring", Year = 2024, Grade = 9 }
            );


        }
    }

}
