using Microsoft.AspNetCore.Mvc;
using School.Models;

namespace School.Controllers
{
    public class TeacherController : Controller
    {
        
        private readonly TeacherAPIController _teacherapi;

        public TeacherController(TeacherAPIController teacherapi)
        {
            _teacherapi = teacherapi;
        }

        // GET : Teacher/New
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            if (ModelState.IsValid)
            {
                int TeacherId = _teacherapi.AddTeacher(NewTeacher);

                if (TeacherId > 0)
                {
                    // redirects to "List" action
                    return RedirectToAction("List");
                }
                else
                {
                    
                    if (string.IsNullOrEmpty(NewTeacher.TeacherFName) || string.IsNullOrEmpty(NewTeacher.TeacherLName))
                    {
                        ModelState.AddModelError("TeacherName", "Teacher name cannot be empty");
                    }
                    if (NewTeacher.TeacherHireDate > DateTime.Now)
                    {
                        ModelState.AddModelError("TeacherHireDate", "Hire date cannot be in the future");
                    }
                    if (!NewTeacher.TeacherEmployeeNumber.StartsWith("T") || !System.Text.RegularExpressions.Regex.IsMatch(NewTeacher.TeacherEmployeeNumber.Substring(1), @"^\d+$"))
                    {
                        ModelState.AddModelError("TeacherEmployeeNumber", "Employee number must start with 'T' followed by digits");
                    }

                   
                    ModelState.AddModelError("", "Failed to add teacher. Employee number may already be taken.");
                }
            }

           
            return View("New", NewTeacher);
        }

        // GET : Teacher/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _teacherapi.FindTeacher(id);

            // Check if teacher exists
            if (SelectedTeacher.TeacherId == 0)
            {
                return RedirectToAction("List"); // Teacher not found
            }

            return View(SelectedTeacher);
        }

        // POST: Teacher/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int RowsAffected = _teacherapi.DeleteTeacher(id);

            if (RowsAffected == 0)
            {
                // Add error message if needed
                TempData["ErrorMessage"] = "Teacher could not be deleted. The teacher may not exist.";
            }

            // redirects to list action
            return RedirectToAction("List");
        }

        // GET: Teacher/List
        [HttpGet]
        public IActionResult List()
        {
          
            return View();
        }
    }
}
