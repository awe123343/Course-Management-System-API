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