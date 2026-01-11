using System;
using System.Collections.Generic;

namespace WorkShop1.Models.ViewModels
{
    public class StudentCoursesVM
    {
        public List<StudentCourseRowVM> Rows { get; set; } = new();
    }

    public class StudentCourseRowVM
    {

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";

        public int? Year { get; set; }
        public string? Semester { get; set; }

        public int? ExamPoints { get; set; }
        public int? SeminarPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }
        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}