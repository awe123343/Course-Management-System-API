using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Materials
{
    public class GradeAssignmentModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Grade { get; set; }
    }
}