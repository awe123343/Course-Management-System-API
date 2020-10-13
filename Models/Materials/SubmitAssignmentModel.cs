using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Materials
{
    public class SubmitAssignmentModel
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int CourseMaterialId { get; set; }
        [Required]
        public string Submission { get; set; }
    }
}