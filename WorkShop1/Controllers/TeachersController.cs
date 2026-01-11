using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkShop1.Data;
using WorkShop1.Models;
using WorkShop1.Models.ViewModels;

namespace WorkShop1.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeachersController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // =========================
        // ADMIN CRUD (unchanged)
        // =========================

        public async Task<IActionResult> Index(string? firstName, string? lastName, string? degree, string? academicRank)
        {
            var teachers = _context.Teachers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(firstName))
                teachers = teachers.Where(t => t.FirstName.Contains(firstName));

            if (!string.IsNullOrWhiteSpace(lastName))
                teachers = teachers.Where(t => t.LastName.Contains(lastName));

            if (!string.IsNullOrWhiteSpace(degree))
                teachers = teachers.Where(t => t.Degree != null && t.Degree.Contains(degree));

            if (!string.IsNullOrWhiteSpace(academicRank))
                teachers = teachers.Where(t => t.AcademicRank != null && t.AcademicRank.Contains(academicRank));

            teachers = teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName);

            return View(await teachers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null) return NotFound();

            var linkedUser = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TeacherId == teacher.Id);
            ViewBag.LinkedUserEmail = linkedUser?.Email;

            return View(teacher);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (!ModelState.IsValid) return View(teacher);

            // pfp image upload
            if (teacher.ProfileImage != null && teacher.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/teachers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(teacher.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await teacher.ProfileImage.CopyToAsync(stream);

                teacher.ImageUrl = "/uploads/teachers/" + fileName;
            }

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id) return NotFound();
            if (!ModelState.IsValid) return View(teacher);

            try
            {
                // pfp image upload if new one is selected
                if (teacher.ProfileImage != null && teacher.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/teachers");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(teacher.ProfileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await teacher.ProfileImage.CopyToAsync(stream);

                    teacher.ImageUrl = "/uploads/teachers/" + fileName;
                }
                else
                {
                    // keep existing image
                    var existingTeacher = await _context.Teachers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == teacher.Id);

                    if (existingTeacher != null)
                        teacher.ImageUrl = existingTeacher.ImageUrl;
                }

                _context.Update(teacher);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Teachers.Any(e => e.Id == teacher.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // TEACHER: My Courses
        // =========================

        [Authorize(Roles = "Teacher")]
        [HttpGet("/TeacherCourses")]
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();

            int teacherId = user.TeacherId.Value;

            var courses = await _context.Courses
                .Where(c => c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId)
                .OrderBy(c => c.Title)
                .ToListAsync();

            return View(courses); // Views/Teachers/MyCourses.cshtml
        }

        // =========================
        // TEACHER: Course Details + Year filter + Students status
        // =========================

        [Authorize(Roles = "Teacher")]
        [HttpGet("/TeacherCourses/Details/{id:int}")]
        public async Task<IActionResult> MyCourseDetails(int id, int? year)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();
            int teacherId = user.TeacherId.Value;

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId));

            if (course == null) return NotFound();

            // години за dropdown (само тие што постојат во enrollments, Year != null)
            var years = await _context.Enrollments
                .Where(e => e.CourseId == id && e.Year.HasValue)
                .Select(e => e.Year!.Value)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // default: најновата година што постои, ако нема -> тековна година
            int selectedYear = year ?? (years.FirstOrDefault() != 0 ? years.FirstOrDefault() : DateTime.Now.Year);

            if (!years.Any())
            {
                years.Add(DateTime.Now.Year);
                selectedYear = DateTime.Now.Year;
            }
            else if (!years.Contains(selectedYear))
            {
                selectedYear = years.First(); // најнова
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == id && e.Year == selectedYear)
                .OrderBy(e => e.Student.LastName)
                .ThenBy(e => e.Student.FirstName)
                .ToListAsync();

            var vm = new TeacherCourseDetailsVM
            {
                Course = course,
                SelectedYear = selectedYear,
                Years = years,
                Rows = enrollments.Select(e => new EnrollmentRowVM
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentDisplay = $"{e.Student.StudentId} - {e.Student.FirstName} {e.Student.LastName}",
                    IsActive = e.FinishDate == null,

                    Grade = e.Grade,
                    FinishDate = e.FinishDate,

                    // points
                    ExamPoints = e.ExamPoints,
                    SeminarPoints = e.SeminarPoints,
                    ProjectPoints = e.ProjectPoints,
                    AdditionalPoints = e.AdditionalPoints,

                    // read-only URLs
                    SeminarUrl = e.SeminarUrl,
                    SeminarOriginalName = e.SeminarOriginalName,
                    ProjectUrl = e.ProjectUrl
                }).ToList()
            };

            return View(vm); // Views/Teachers/MyCourseDetails.cshtml
        }

        // =========================
        // TEACHER: Update points/grade/finishdate ONLY for active enrollments
        // =========================

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrollment(TeacherUpdateEnrollmentVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();
            int teacherId = user.TeacherId.Value;

            // провери дека предметот е негов
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == vm.CourseId &&
                    (c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId));

            if (course == null) return Forbid();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == vm.EnrollmentId
                                       && e.CourseId == vm.CourseId
                                       && e.Year == vm.Year);

            if (enrollment == null) return NotFound();

            // само активен може да се менува
            if (enrollment.FinishDate != null)
                return RedirectToAction(nameof(MyCourseDetails), new { id = vm.CourseId, year = vm.Year });

            // update editable fields
            enrollment.ExamPoints = vm.ExamPoints;
            enrollment.SeminarPoints = vm.SeminarPoints;
            enrollment.ProjectPoints = vm.ProjectPoints;
            enrollment.AdditionalPoints = vm.AdditionalPoints;

            enrollment.Grade = vm.Grade;
            enrollment.FinishDate = vm.FinishDate;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCourseDetails), new { id = vm.CourseId, year = vm.Year });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher(string email, int teacherId)
        {
            email = (email ?? "").Trim();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound("Teacher not found");

            var otherUser = await _userManager.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.TeacherId == teacherId);

            if (otherUser != null && otherUser.Id != user.Id)
            {
                TempData["Error"] = $"TeacherId={teacherId} веќе е врзан со {otherUser.Email}";
                return RedirectToAction(nameof(Details), new { id = teacherId });
            }

            user.TeacherId = teacherId;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                TempData["Error"] = string.Join(" | ", update.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Details), new { id = teacherId });
            }

            if (!await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                var addRole = await _userManager.AddToRoleAsync(user, "Teacher");
                if (!addRole.Succeeded)
                {
                    TempData["Error"] = "AddToRole(Teacher) failed: " +
                                        string.Join(" | ", addRole.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Details), new { id = teacherId });
                }
            }

            TempData["Success"] = "Assigned + role OK.";
            return RedirectToAction(nameof(Details), new { id = teacherId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignTeacher(int teacherId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.TeacherId == teacherId);

            if (user == null)
            {
                TempData["Error"] = "Овој наставник не е поврзан со ниеден user.";
                return RedirectToAction(nameof(Details), new { id = teacherId });
            }

            user.TeacherId = null;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                TempData["Error"] = string.Join(" | ", update.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Details), new { id = teacherId });
            }

            TempData["Success"] = $"Unassigned: {user.Email}";
            return RedirectToAction(nameof(Details), new { id = teacherId });
        }
    }
}