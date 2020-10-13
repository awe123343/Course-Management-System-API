namespace CourseSysAPI.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Capacity { get; set; }
        public int EvaluatorId { get; set; }
        public string Description { get; set; }
    }
}