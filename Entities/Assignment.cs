namespace CourseSysAPI.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseMaterialId { get; set; }
        public string Submission { get; set; }
        public int? Grades { get; set; }
    }
}