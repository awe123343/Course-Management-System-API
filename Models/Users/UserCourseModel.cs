using CourseSysAPI.Entities;
using System.Collections.Generic;

namespace CourseSysAPI.Models.Users
{
    public class UserCourseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public List<Course> Courses { get; set; }
    }
}