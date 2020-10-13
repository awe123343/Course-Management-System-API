namespace CourseSysAPI.Entities
{
    public class CourseMaterial
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsAssignment { get; set; }
    }
}