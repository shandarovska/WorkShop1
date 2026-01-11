using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShop1.Models.ViewModels
{
    public class StudentEnrollmentEditVM
    {
        [Required]
        public long EnrollmentId { get; set; }

        // read-only info
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public int? Year { get; set; }
        public string? Semester { get; set; }

        // editable by student
        [StringLength(255)]
        public string? ProjectUrl { get; set; }

        // seminar file upload
        public IFormFile? SeminarFile { get; set; }

        // read-only preview
        public string? SeminarUrl { get; set; }
        public string? SeminarOriginalName { get; set; }

        // status read-only
        public int? ExamPoints { get; set; }
        public int? SeminarPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }
        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}