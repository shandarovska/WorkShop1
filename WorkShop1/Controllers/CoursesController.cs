using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkShop1.Data;
using WorkShop1.Models;
using WorkShop1.Models.ViewModels;

namespace WorkShop1.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: Courses
        public async Task<IActionResult> Index(string title,
        int? semester,
        string programme,
        int? teacherId)
        {
            var courses = _context.Courses
             .Include(c => c.FirstTeacher)
             .Include(c => c.SecondTeacher)
             .AsQueryable();

            //filteri
            if (!string.IsNullOrEmpty(title))
            {
                courses = courses.Where(c =>
                    c.Title.Contains(title));
            }

            if (semester.HasValue)
            {
                courses = courses.Where(c =>
                    c.Semester == semester);
            }

            if (!string.IsNullOrEmpty(programme))
            {
                courses = courses.Where(c =>
                    c.Programme != null && c.Programme.Contains(programme));
            }

            if (teacherId.HasValue)
            {
                courses = courses.Where(c =>
                    c.FirstTeacherId == teacherId ||
                    c.SecondTeacherId == teacherId);
            }

            // dropdown data
            ViewBag.Teachers = await _context.Teachers.ToListAsync();

            return View(await courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var teachers = GetTeachersForDropdown();
            ViewData["FirstTeacherId"] = new SelectList(teachers, "Id", "FullName");
            ViewData["SecondTeacherId"] = new SelectList(teachers, "Id", "FullName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private List<object> GetTeachersForDropdown()
        {
            return _context.Teachers
                .Select(t => new
                {
                    t.Id,
                    FullName = t.FirstName + " " + t.LastName
                })
                .Cast<object>()
                .ToList();
        }
        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudents(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            int year = DateTime.Now.Year;
            string semester = "Winter";

            // current enrollments for this course + period
            var currentEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == id
                            && e.Year == year
                            && e.Semester == semester)
                .OrderBy(e => e.Student.LastName)
                .ThenBy(e => e.Student.FirstName)
                .ToListAsync();

            var enrolledStudentIds = currentEnrollments
                .Select(e => e.StudentId)
                .ToHashSet();

            var vm = new CourseEnrollVM
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                Year = year,
                Semester = semester,

                CurrentEnrollments = currentEnrollments.Select(e => new CurrentEnrollmentVM
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentDisplayName = $"{e.Student.StudentId} - {e.Student.FirstName} {e.Student.LastName}",
                    FinishDate = e.FinishDate,
                    Grade = e.Grade
                }).ToList(),

                Students = await _context.Students
                    .OrderBy(s => s.LastName)
                    .ThenBy(s => s.FirstName)
                    .Select(s => new StudentEnrollItemVM
                    {
                        StudentId = s.Id,
                        DisplayName = $"{s.StudentId} - {s.FirstName} {s.LastName}",
                        Selected = false,
                        AlreadyEnrolled = enrolledStudentIds.Contains(s.Id)
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudents(CourseEnrollVM vm)
        {
            var selectedStudentIds = vm.Students
                .Where(s => s.Selected && !s.AlreadyEnrolled)
                .Select(s => s.StudentId)
                .ToList();

            if (!selectedStudentIds.Any())
            {
                ModelState.AddModelError("", "Select at least one student to enroll.");
            }

            if (!ModelState.IsValid)
            {
                // re-load course
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == vm.CourseId);
                vm.CourseTitle = course?.Title ?? "";

                // reload current enrollments
                var currentEnrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == vm.CourseId
                                && e.Year == vm.Year
                                && e.Semester == vm.Semester)
                    .ToListAsync();

                var enrolledStudentIds = currentEnrollments
                    .Select(e => e.StudentId)
                    .ToHashSet();

                vm.CurrentEnrollments = currentEnrollments.Select(e => new CurrentEnrollmentVM
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentDisplayName = $"{e.Student.StudentId} - {e.Student.FirstName} {e.Student.LastName}",
                    FinishDate = e.FinishDate,
                    Grade = e.Grade
                }).ToList();

                vm.Students = await _context.Students
                    .OrderBy(s => s.LastName)
                    .ThenBy(s => s.FirstName)
                    .Select(s => new StudentEnrollItemVM
                    {
                        StudentId = s.Id,
                        DisplayName = $"{s.StudentId} - {s.FirstName} {s.LastName}",
                        Selected = selectedStudentIds.Contains(s.Id),
                        AlreadyEnrolled = enrolledStudentIds.Contains(s.Id)
                    })
                    .ToListAsync();

                return View(vm);
            }



            foreach (var sid in selectedStudentIds)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    CourseId = vm.CourseId,
                    StudentId = sid,
                    Year = vm.Year,
                    Semester = vm.Semester,
                    // all other fields intentionally NULL
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EnrollStudents), new { id = vm.CourseId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateEnrollments(CourseEnrollVM vm)
        {
            if (vm.SelectedEnrollmentIds == null || vm.SelectedEnrollmentIds.Count == 0)
            {
                TempData["EnrollMsg"] = "Select at least one enrolled student to deactivate.";
                return RedirectToAction(nameof(EnrollStudents), new { id = vm.CourseId });
            }

            if (vm.FinishDateForDeactivation == null)
            {
                TempData["EnrollMsg"] = "Select finish date.";
                return RedirectToAction(nameof(EnrollStudents), new { id = vm.CourseId });
            }

            var enrollments = await _context.Enrollments
                .Where(e => vm.SelectedEnrollmentIds.Contains(e.Id))
                .ToListAsync();

            foreach (var e in enrollments)
            {
                e.FinishDate = vm.FinishDateForDeactivation; // deactivate
            }

            await _context.SaveChangesAsync();
            TempData["EnrollMsg"] = "Deactivated selected students.";
            return RedirectToAction(nameof(EnrollStudents), new { id = vm.CourseId });
        }
       

       
    }


}
