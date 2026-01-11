using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkShop1.Models;

namespace WorkShop1.Models
{
    public class Enrollment
    {
        public long Id { get; set; }

        // FK
        public int CourseId { get; set; }
        [ValidateNever]
        public Course Course { get; set; } = null!;

        public long StudentId { get; set; }
        [ValidateNever]
        public Student Student { get; set; } = null!;

        [StringLength(10)]
        public string? Semester { get; set; }

        public int? Year { get; set; }
        public int? Grade { get; set; }

        [StringLength(255)]
        public string? SeminarUrl { get; set; }

        [NotMapped]
        public IFormFile? SeminarFile { get; set; }

        public string? SeminarOriginalName { get; set; }


        [StringLength(255)]
        public string? ProjectUrl { get; set; }

        public int? ExamPoints { get; set; }
        public int? SeminarPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }

        public DateTime? FinishDate { get; set; }
    }
}