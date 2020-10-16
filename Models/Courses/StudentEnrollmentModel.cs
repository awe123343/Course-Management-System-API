using System.ComponentModel.DataAnnotations;

namespace CourseSysAPI.Models.Courses
{
    public class StudentEnrollmentModel
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Capacity { get; set; }
        public string EvaluatorName { get; set; }
        public string Description { get; set; }
        public int CurrentStuNo { get; set; }
        public int EnrollmentId { get; set; }
    }
}