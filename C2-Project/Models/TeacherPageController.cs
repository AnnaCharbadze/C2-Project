using Microsoft.AspNetCore.Mvc;
using School.Models;
namespace School.Controllers
{
    public class TeacherPageController : Controller
    {
       
        private readonly TeacherAPIController _teacherapi;
        public TeacherPageController(TeacherAPIController teacherapi)
        {
            _teacherapi = teacherapi;
        }
        // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        // POST: TeacherPage/Create
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
                    // Check for specific validation errors and add them to ModelState
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
                    // Add a general error for duplicate employee number
                    ModelState.AddModelError("", "Failed to add teacher. Employee number may already be taken.");
                }
            }
            // If  something went wrong 
            return View("New", NewTeacher);
        }
        // GET : TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _teacherapi.FindTeacher(id);
            // Check if teacher exists
            if (SelectedTeacher.TeacherId == 0)
            {
                return RedirectToAction("List"); 
            }
            return View(SelectedTeacher);
        }
        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int RowsAffected = _teacherapi.DeleteTeacher(id);
            if (RowsAffected == 0)
            {
                // Add error message if needed
                TempData["ErrorMessage"] = "Teacher could not be deleted.";
            }
            // redirects to list action
            return RedirectToAction("List");
        }
        // GET: TeacherPage/List
        [HttpGet]
        public IActionResult List()
        {
    
            return View();
        }

        // GET : TeacherPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _teacherapi.FindTeacher(id);
            
            // Check if teacher exists
            if (SelectedTeacher.TeacherId == 0)
            {
                TempData["ErrorMessage"] = "Teacher not found.";
                return RedirectToAction("List");
            }
            
            return View(SelectedTeacher);
        }

        // POST: TeacherPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, Teacher UpdatedTeacher)
        {
            if (ModelState.IsValid)
            {
               
                bool isValid = true;
                
                // Validate teacher name
                if (string.IsNullOrEmpty(UpdatedTeacher.TeacherFName) || string.IsNullOrEmpty(UpdatedTeacher.TeacherLName))
                {
                    ModelState.AddModelError("TeacherName", "Teacher name cannot be empty");
                    isValid = false;
                }
                
                // Validate hire date
                if (UpdatedTeacher.TeacherHireDate > DateTime.Now)
                {
                    ModelState.AddModelError("TeacherHireDate", "Hire date cannot be in the future");
                    isValid = false;
                }
                
                // Validate employee number
                if (string.IsNullOrEmpty(UpdatedTeacher.TeacherEmployeeNumber) ||
                    !UpdatedTeacher.TeacherEmployeeNumber.StartsWith("T") ||
                    !System.Text.RegularExpressions.Regex.IsMatch(UpdatedTeacher.TeacherEmployeeNumber.Substring(1), @"^\d+$"))
                {
                    ModelState.AddModelError("TeacherEmployeeNumber", "Employee number must start with 'T' followed by digits");
                    isValid = false;
                }
                
                // Validate salary
                if (UpdatedTeacher.TeacherSalary < 0)
                {
                    ModelState.AddModelError("TeacherSalary", "Salary cannot be negative");
                    isValid = false;
                }
                
                if (isValid)
                {
                    // Call API to update teacher
                    Teacher result = _teacherapi.UpdateTeacher(id, UpdatedTeacher);
                    
                    // Check if update was successful
                    if (result.TeacherId > 0)
                    {
                        TempData["SuccessMessage"] = "Teacher updated successfully.";
                        return RedirectToAction("List");
                    }
                    else
                    {
                        // Add a general error for API failures
                        ModelState.AddModelError("", "Failed to update teacher.");
                    }
                }
            }
            
            // If something went wrong 
            return View("Edit", UpdatedTeacher);
        }
    }
}
