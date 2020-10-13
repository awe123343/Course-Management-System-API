using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Courses
{
    public class EnrollCourseModel
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}