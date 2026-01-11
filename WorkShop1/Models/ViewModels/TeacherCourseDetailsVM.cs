using System;
using System.Collections.Generic;
using WorkShop1.Models;

namespace WorkShop1.Models.ViewModels
{
    public class TeacherCourseDetailsVM
    {
        public Course Course { get; set; } = null!;
        public int SelectedYear { get; set; }
        public List<int> Years { get; set; } = new();

        public List<EnrollmentRowVM> Rows { get; set; } = new();
    }

    public class EnrollmentRowVM
    {
        public long EnrollmentId { get; set; }   // Enrollment.Id е long
        public long StudentId { get; set; }      // Enrollment.StudentId е long

        public string StudentDisplay { get; set; } = "";
        public bool IsActive { get; set; }

        // Editable points (како во Enrollment)
        public int? ExamPoints { get; set; }
        public int? SeminarPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }

        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }

        // Read-only URLs (како во Enrollment)
        public string? SeminarUrl { get; set; }
        public string? SeminarOriginalName { get; set; }
        public string? ProjectUrl { get; set; }
    }
}