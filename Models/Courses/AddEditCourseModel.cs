using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Courses
{
    public class AddEditCourseModel
    {
        [Required]
        public string CourseCode { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int EvaluatorId { get; set; }
        public string Description { get; set; }
    }
}