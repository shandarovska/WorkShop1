using System.ComponentModel.DataAnnotations;

namespace WorkShop1.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = null!;

        public int Credits { get; set; }
        public int Semester { get; set; }

        [StringLength(100)]
        public string? Programme { get; set; }

        [StringLength(25)]
        public string? EducationLevel { get; set; }

        // profesori
        public int? FirstTeacherId { get; set; }
        public Teacher? FirstTeacher { get; set; }

        public int? SecondTeacherId { get; set; }
        public Teacher? SecondTeacher { get; set; }

        // studenti
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
