using System;
using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Teacher Employee Number is required")]
        public string TeacherEmployeeNumber { get; set; }

        [Required(ErrorMessage = "Teacher First Name is required")]
        public string TeacherFName { get; set; }

        [Required(ErrorMessage = "Teacher Last Name is required")]
        public string TeacherLName { get; set; }

        public DateTime TeacherHireDate { get; set; }

        public decimal TeacherSalary { get; set; }
    }
}
