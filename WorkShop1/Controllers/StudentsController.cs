using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkShop1.Data;
using WorkShop1.Models;
using WorkShop1.Models.ViewModels;
using WorkShop1.Data;
using WorkShop1.Models;
using WorkShop1.Models.ViewModels;

namespace WorkShop1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // =========================
        // ADMIN CRUD (your existing)
        // =========================

        // GET: Students
        public async Task<IActionResult> Index(string? index, string? firstName, string? lastName, int? courseId)
        {
            ViewBag.Courses = new SelectList(
                 await _context.Courses
                    .OrderBy(c => c.Title)
                    .Select(c => new { c.Id, c.Title })
                    .ToListAsync(),
                "Id",
                "Title",
                courseId
            );

            var students = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(index))
            {
                students = students.Where(s => s.StudentId.Contains(index));
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                students = students.Where(s => s.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                students = students.Where(s => s.LastName.Contains(lastName));
            }

            if (courseId.HasValue)
            {
                students = students.Where(s =>
                    _context.Enrollments.Any(e =>
                        e.StudentId == s.Id &&
                        e.CourseId == courseId.Value));
            }

            students = students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName);

            return View(await students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
                return NotFound();

            var linkedUser = await _userManager.Users
    .AsNoTracking()
    .FirstOrDefaultAsync(u => u.StudentId == student.Id);

            ViewBag.LinkedUserEmail = linkedUser?.Email;


            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            // pfp image upload
            if (student.ProfileImage != null && student.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/students");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(student.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await student.ProfileImage.CopyToAsync(stream);

                student.ImageUrl = "/uploads/students/" + fileName;
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, Student student)
        {
            if (id != student.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(student);

            try
            {
                // profile image upload (if new)
                if (student.ProfileImage != null && student.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/students");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(student.ProfileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await student.ProfileImage.CopyToAsync(stream);

                    student.ImageUrl = "/uploads/students/" + fileName;
                }
                else
                {
                    // keep existing image
                    var existingStudent = await _context.Students
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == student.Id);

                    if (existingStudent != null)
                        student.ImageUrl = existingStudent.ImageUrl;
                }

                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
                _context.Students.Remove(student);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        // =========================
        // STUDENT FUNCTIONALITIES
        // =========================

        // GET: /StudentCourses
        [Authorize(Roles = "Student")]
        [HttpGet("/StudentCourses")]
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
                return Forbid();

            long studentId = user.StudentId.Value;

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.Year)
                .ThenBy(e => e.Course.Title)
                .ToListAsync();

            var vm = new StudentCoursesVM
            {
                Rows = enrollments.Select(e => new StudentCourseRowVM
                {
                    
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    Year = e.Year,
                    Semester = e.Semester,

                    ExamPoints = e.ExamPoints,
                    SeminarPoints = e.SeminarPoints,
                    ProjectPoints = e.ProjectPoints,
                    AdditionalPoints = e.AdditionalPoints,
                    Grade = e.Grade,
                    FinishDate = e.FinishDate
                }).ToList()
            };

            return View(vm); // Views/Students/MyCourses.cshtml
        }

        // GET: /StudentCourses/Details/{courseId}
        [Authorize(Roles = "Student")]
        [HttpGet("/StudentCourses/Details/{courseId:int}")]
        public async Task<IActionResult> MyCourseDetails(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
                return Forbid();

            long studentId = user.StudentId.Value;

            // take the latest enrollment for that course (latest year)
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .OrderByDescending(e => e.Year)
                .FirstOrDefaultAsync();

            if (enrollment == null)
                return NotFound();

            var vm = new StudentEnrollmentEditVM
            {
                

                CourseId = enrollment.CourseId,
                CourseTitle = enrollment.Course.Title,
                Year = enrollment.Year,
                Semester = enrollment.Semester,

                ProjectUrl = enrollment.ProjectUrl,
                SeminarUrl = enrollment.SeminarUrl,
                SeminarOriginalName = enrollment.SeminarOriginalName,

                ExamPoints = enrollment.ExamPoints,
                SeminarPoints = enrollment.SeminarPoints,
                ProjectPoints = enrollment.ProjectPoints,
                AdditionalPoints = enrollment.AdditionalPoints,
                Grade = enrollment.Grade,
                FinishDate = enrollment.FinishDate
            };

            return View(vm); // Views/Students/MyCourseDetails.cshtml
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudent(string email, long studentId)
        {
            email = (email ?? "").Trim();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found");

            // студентот да не е врзан на друг user
            var otherUser = await _userManager.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.StudentId == studentId);

            if (otherUser != null && otherUser.Id != user.Id)
            {
                TempData["Error"] = $"StudentId={studentId} веќе е врзан со {otherUser.Email}";
                return RedirectToAction(nameof(Details), new { id = studentId });
            }

            user.StudentId = studentId;
            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                TempData["Error"] = string.Join(" | ", update.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Details), new { id = studentId });
            }

            if (!await _userManager.IsInRoleAsync(user, "Student"))
            {
                var addRole = await _userManager.AddToRoleAsync(user, "Student");
                if (!addRole.Succeeded)
                {
                    TempData["Error"] = "AddToRole(Student) failed: " +
                                        string.Join(" | ", addRole.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Details), new { id = studentId });
                }
            }

            TempData["Success"] = "Assigned + role OK.";
            return RedirectToAction(nameof(Details), new { id = studentId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignStudent(long studentId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null)
            {
                TempData["Error"] = "Овој студент не е поврзан со ниеден user.";
                return RedirectToAction(nameof(Details), new { id = studentId });
            }

            user.StudentId = null;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                TempData["Error"] = string.Join(" | ", update.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Details), new { id = studentId });
            }

            TempData["Success"] = $"Unassigned: {user.Email}";
            return RedirectToAction(nameof(Details), new { id = studentId });
        }



        // POST: Students can update ONLY URLs / seminar file
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrollment(StudentEnrollmentEditVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
                return Forbid();

            long studentId = user.StudentId.Value;

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == vm.EnrollmentId && e.StudentId == studentId);

            if (enrollment == null)
                return NotFound();

            // project URL (GitHub)
            enrollment.ProjectUrl = vm.ProjectUrl;

            // seminar file upload (.doc/.docx/.pdf)
            if (vm.SeminarFile != null && vm.SeminarFile.Length > 0)
            {
                var ext = Path.GetExtension(vm.SeminarFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".doc", ".docx", ".pdf" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.SeminarFile), "Allowed formats: .doc, .docx, .pdf");
                }
                else
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "seminars");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"seminar_{enrollment.Id}_{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await vm.SeminarFile.CopyToAsync(stream);

                    enrollment.SeminarUrl = "/uploads/seminars/" + fileName;
                    enrollment.SeminarOriginalName = vm.SeminarFile.FileName;
                }
            }

            if (!ModelState.IsValid)
            {
                // refill read-only properties for the view
                vm.CourseId = enrollment.CourseId;
                vm.CourseTitle = enrollment.Course.Title;
                vm.Year = enrollment.Year;
                vm.Semester = enrollment.Semester;

                vm.SeminarUrl = enrollment.SeminarUrl;
                vm.SeminarOriginalName = enrollment.SeminarOriginalName;

                vm.ExamPoints = enrollment.ExamPoints;
                vm.SeminarPoints = enrollment.SeminarPoints;
                vm.ProjectPoints = enrollment.ProjectPoints;
                vm.AdditionalPoints = enrollment.AdditionalPoints;
                vm.Grade = enrollment.Grade;
                vm.FinishDate = enrollment.FinishDate;

                return View("MyCourseDetails", vm);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCourseDetails), new { courseId = enrollment.CourseId });
        }

    }

}
