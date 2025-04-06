using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using School.Models;
using System;

namespace School.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a teacher to the database
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <returns>
        /// The inserted Teacher Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            // Error handling for empty teacher name
            if (string.IsNullOrEmpty(TeacherData.TeacherFName) || string.IsNullOrEmpty(TeacherData.TeacherLName))
            {
                return 0;
            }

            // Error handling for employee number format
            if (string.IsNullOrEmpty(TeacherData.TeacherEmployeeNumber) ||
                !TeacherData.TeacherEmployeeNumber.StartsWith("T") ||
                !System.Text.RegularExpressions.Regex.IsMatch(TeacherData.TeacherEmployeeNumber.Substring(1), @"^\d+$"))
            {
                return 0;
            }

            // Error handling for future hire date
            if (TeacherData.TeacherHireDate > DateTime.Now)
            {
                return 0;
            }

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, hiredate, salary, employeenumber) VALUES (@teacherfname, @teacherlname, @hiredate, @salary, @employeenumber)";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.TeacherHireDate);
                Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.TeacherEmployeeNumber);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);
            }
            return 0;
        }

        /// <summary>
        /// Deletes a Teacher from the database
        /// </summary>
        /// <param name="TeacherId">Primary key of the teacher to delete</param>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete(template: "DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            // Check if teacher exists before attempting to delete
            bool teacherExists = false;
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand CheckCommand = Connection.CreateCommand();
                CheckCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                CheckCommand.Parameters.AddWithValue("@id", TeacherId);
                int count = Convert.ToInt32(CheckCommand.ExecuteScalar());
                teacherExists = (count > 0);
            }

            // Error handling for non-existent teacher
            if (!teacherExists)
            {
                return 0;
            }
            {
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();

                    Command.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
                    Command.Parameters.AddWithValue("@id", TeacherId);
                    return Command.ExecuteNonQuery();
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns a teacher in the database by their ID
        /// </summary>
        /// <param name="id">The Teacher ID primary key</param>
        /// <returns>
        /// A matching teacher object by its ID. Empty object if Teacher not found
        /// </returns>
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = new Teacher();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "SELECT teacherid, teacherfname, teacherlname, hiredate, salary, employeenumber FROM teachers WHERE teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        string TeacherFName = ResultSet["teacherfname"].ToString();
                        string TeacherLName = ResultSet["teacherlname"].ToString();
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal TeacherSalary = Convert.ToDecimal(ResultSet["salary"]);
                        string TeacherEmployeeNumber = ResultSet["employeenumber"].ToString();

                        SelectedTeacher.TeacherId = TeacherId;
                        SelectedTeacher.TeacherFName = TeacherFName;
                        SelectedTeacher.TeacherLName = TeacherLName;
                        SelectedTeacher.TeacherHireDate = TeacherHireDate;
                        SelectedTeacher.TeacherSalary = TeacherSalary;
                        SelectedTeacher.TeacherEmployeeNumber = TeacherEmployeeNumber;
                    }
                }
            }

            return SelectedTeacher;
        }
    }
}