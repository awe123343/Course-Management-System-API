using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Materials
{
    public class AddEditMaterialModel
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string content { get; set; }
        [Required]
        public bool IsAssignment { get; set; }
    }
}