using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Materials
{
    public class StuCourseComboModel
    {
        [Required]
        public int StuId { get; set; }
        [Required]
        public int CourseId { get; set; }
    }
}