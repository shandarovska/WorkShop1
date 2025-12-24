using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkShop1.Data;
using WorkShop1.Models;
using System.IO;


namespace WorkShop1.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(int courseId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.Student.LastName)
                .ThenBy(e => e.Student.FirstName)
                .ToListAsync();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = enrollments.FirstOrDefault()?.Course.Title;

            return View(enrollments);
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create(int courseId)
        {
            ViewBag.CourseId = courseId;
            LoadStudentListDropDownMenu();  

            return View();
        }


        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            // 1. Проверка на валидност
            if (!ModelState.IsValid)
            {
                LoadStudentListDropDownMenu(enrollment.StudentId);
                ViewBag.CourseId = enrollment.CourseId;
                return View(enrollment);
            }

            // 2. Проверка дали студентот е веќе запишан на предметот
            var alreadyEnrolled = await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.CourseId == enrollment.CourseId &&
                    e.StudentId == enrollment.StudentId);

            if (alreadyEnrolled != null)
            {
                ModelState.AddModelError(string.Empty,
                    "The selected student is already enrolled in this course.");

                LoadStudentListDropDownMenu(enrollment.StudentId);
                ViewBag.CourseId = enrollment.CourseId;
                return View(enrollment);
            }

            // 3. Зачувување
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = enrollment.CourseId });
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            LoadStudentListDropDownMenu(enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Enrollment enrollment)
        {
            var dbEnrollment = await _context.Enrollments.FindAsync(enrollment.Id);
            if (dbEnrollment == null)
                return NotFound();

            if (enrollment.SeminarFile != null && enrollment.SeminarFile.Length > 0)
            {
                var ext = Path.GetExtension(enrollment.SeminarFile.FileName).ToLower();
                if (ext == ".pdf" || ext == ".doc" || ext == ".docx")
                {
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "enrollments");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await enrollment.SeminarFile.CopyToAsync(stream);

                    dbEnrollment.SeminarUrl = "/uploads/enrollments/" + fileName;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Students",
                new { id = dbEnrollment.StudentId }
            );
        }


        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                int courseId = enrollment.CourseId;
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { courseId });
            }
            return RedirectToAction(nameof(Index));
        }

        //dropdown celoime
        private void LoadStudentListDropDownMenu(long? selectedStudentId = null)
        {
            ViewData["StudentId"] = new SelectList(
                _context.Students
                    .OrderBy(s => s.LastName)
                    .ThenBy(s => s.FirstName)
                    .Select(s => new
                    {
                        s.Id,
                        FullName = s.FirstName + " " + s.LastName + " (" + s.StudentId + ")"
                    })
                    .ToList(),
                "Id",
                "FullName",
                selectedStudentId
            );
        }
       

        private bool EnrollmentExists(long id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
