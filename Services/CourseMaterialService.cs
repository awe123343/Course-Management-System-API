using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using CourseSysAPI.Data;
using CourseSysAPI.Entities;
using CourseSysAPI.Helpers;
using CourseSysAPI.Models.Courses;
using AutoMapper;

namespace CourseSysAPI.Services
{
    public interface ICourseMaterialService
    {
        IEnumerable<CourseMaterial> GetAll();
        IEnumerable<CourseMaterial> GetByCourse(int id);
        IEnumerable<CourseMaterial> GetByEvaluator(int id);
        IEnumerable<CourseMaterial> GetByStudent(int id);
        IEnumerable<Assignment> GetAssignmentByEvaluaotr(int evaluatorId);
        IEnumerable<Assignment> GetAssignmentByStudent(int stuId);
        IEnumerable<Assignment> GetAssignmentByStudentCourse(int stuId, int courseId);
        CourseMaterial GetById(int id);
        CourseMaterial Create(CourseMaterial courseMaterial);
        void Update(CourseMaterial newCourseMaterial);
        void Delete(int id);
        Assignment SubmitAssignment(Assignment assignment);
        void UpdateAssignment(Assignment newAssignment);
        void GradeAssignment(int id, int grade);
    }

    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public CourseMaterialService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<CourseMaterial> GetAll()
        {
            return _context.CourseMaterials;
        }

        public IEnumerable<CourseMaterial> GetByCourse(int id)
        {
            return _context.CourseMaterials.Where(material => material.CourseId == id);
        }

        public IEnumerable<CourseMaterial> GetByEvaluator(int id)
        {
            HashSet<int> evaluatorCourseIds = new HashSet<int>(_context.Courses.Where(c => c.EvaluatorId == id).Select(c => c.Id));
            return _context.CourseMaterials.Where(material => evaluatorCourseIds.Contains(material.CourseId));
        }

        public IEnumerable<CourseMaterial> GetByStudent(int id)
        {
            HashSet<int> courseStudentTake = new HashSet<int>(_context.Enrollments.Where(enroll => enroll.StudentId == id).Select(enroll => enroll.CourseId));
            return _context.CourseMaterials.Where(material => courseStudentTake.Contains(material.CourseId));
        }

        public IEnumerable<Assignment> GetAssignmentByEvaluaotr(int evaluatorId)
        {
            HashSet<int> assignmentIdsForEvaluator = new HashSet<int>(GetByEvaluator(evaluatorId).Where(assignment => assignment.IsAssignment).Select(assignment => assignment.Id));
            return _context.Assignments.Where(assignment => assignmentIdsForEvaluator.Contains(assignment.CourseMaterialId));
        }

        public IEnumerable<Assignment> GetAssignmentByStudent(int stuId)
        {
            return _context.Assignments.Where(assignment => (assignment.StudentId == stuId));
        }

        public IEnumerable<Assignment> GetAssignmentByStudentCourse(int stuId, int courseId)
        {
            HashSet<int> currCourseAssignment = new HashSet<int>(_context.CourseMaterials.Where(cm => (cm.CourseId == courseId) && (cm.IsAssignment)).Select(cm => cm.Id));
            return _context.Assignments.Where(assignment => (assignment.StudentId == stuId) && (currCourseAssignment.Contains(assignment.CourseMaterialId)));
        }

        public CourseMaterial GetById(int id)
        {
            return _context.CourseMaterials.Find(id);
        }

        public CourseMaterial Create(CourseMaterial courseMaterial)
        {
            _context.CourseMaterials.Add(courseMaterial);
            _context.SaveChanges();

            return courseMaterial;
        }

        public void Update(CourseMaterial newCourseMaterial)
        {
            var courseMaterial = _context.CourseMaterials.Find(newCourseMaterial.Id);

            if (courseMaterial == null)
                throw new AppException("Course material not found");

            // update course material properties if provided
            if (!string.IsNullOrWhiteSpace(newCourseMaterial.Title) && newCourseMaterial.Title != courseMaterial.Title)
            {
                courseMaterial.Title = newCourseMaterial.Title;
            }

            if (!string.IsNullOrWhiteSpace(newCourseMaterial.Content) && newCourseMaterial.Content != courseMaterial.Content)
            {
                courseMaterial.Content = newCourseMaterial.Content;
            }

            courseMaterial.IsAssignment = newCourseMaterial.IsAssignment;

            _context.CourseMaterials.Update(courseMaterial);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var courseMaterial = _context.CourseMaterials.Find(id);
            if (courseMaterial != null)
            {
                _context.CourseMaterials.Remove(courseMaterial);
                _context.SaveChanges();
            }
        }

        public Assignment SubmitAssignment(Assignment assignment)
        {
            if (_context.Assignments.Any(x => (x.CourseMaterialId == assignment.CourseMaterialId && x.StudentId == assignment.StudentId)))
                throw new AppException("You already have a submission for assignment.");

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            return assignment;
        }

        public void UpdateAssignment(Assignment newAssignment)
        {
            var assignment = _context.Assignments.Find(newAssignment.Id);

            if (assignment == null)
                throw new AppException("Assignment submission not found");

            assignment.Submission = newAssignment.Submission;

            _context.Assignments.Update(assignment);
            _context.SaveChanges();
        }

        public void GradeAssignment(int id, int grade)
        {
            var assignment = _context.Assignments.Find(id);

            if (assignment == null)
                throw new AppException("Assignment submission not found");

            assignment.Grades = grade;

            _context.Assignments.Update(assignment);
            _context.SaveChanges();
        }
    }
}