namespace WorkShop1.Models.ViewModels
{   public class CourseEnrollVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";

        public int Year { get; set; } = DateTime.Now.Year;
        public string Semester { get; set; } = "Winter"; // Winter / Summer

        // students to enroll (checkboxes)
        public List<StudentEnrollItemVM> Students { get; set; } = new();

        // current enrollments for the same course + year + semester
        public List<CurrentEnrollmentVM> CurrentEnrollments { get; set; } = new();

        public DateTime? FinishDateForDeactivation { get; set; } = DateTime.Today;

        // selected enrollment ids to deactivate
        public List<long> SelectedEnrollmentIds { get; set; } = new();
    }

    public class StudentEnrollItemVM
    {
        public long StudentId { get; set; }
        public string DisplayName { get; set; } = ""; // "201001 - Stefan Mitrev"
        public bool Selected { get; set; }
        public bool AlreadyEnrolled { get; set; } // for UI (disable checkbox)
    }

    public class CurrentEnrollmentVM
    {
        public long EnrollmentId { get; set; }
        public long StudentId { get; set; }
        public string StudentDisplayName { get; set; } = "";
        public DateTime? FinishDate { get; set; } // null = active
        public int? Grade { get; set; }
    }
}
