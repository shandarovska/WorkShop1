using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShop1.Models.ViewModels
{
    public class TeacherUpdateEnrollmentVM
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public long EnrollmentId { get; set; }   // Enrollment.Id е long

        [Required]
        public int Year { get; set; }            // Year го праќаме како int од dropdown

        // Editable points (како во Enrollment)
        public int? ExamPoints { get; set; }
        public int? SeminarPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }

        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}